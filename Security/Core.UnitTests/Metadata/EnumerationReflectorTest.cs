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

    private EnumerationReflector _enumerationReflector;
    private MetadataCache _cache;

    // construction and disposing

    public EnumerationReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumerationReflector = new EnumerationReflector ();
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IEnumerationReflector), _enumerationReflector);
    }

    [Test]
    public void GetValues ()
    {
      Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (typeof (DomainAccessType), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (2, values.Count);

      Assert.AreEqual (0, values[DomainAccessType.Journalize].Value);
      Assert.AreEqual ("Journalize", values[DomainAccessType.Journalize].Name);
      Assert.AreEqual ("00000002-0001-0000-0000-000000000000", values[DomainAccessType.Journalize].ID);
      
      Assert.AreEqual (1, values[DomainAccessType.Archive].Value);
      Assert.AreEqual ("Archive", values[DomainAccessType.Archive].Name);
      Assert.AreEqual ("00000002-0002-0000-0000-000000000000", values[DomainAccessType.Archive].ID);
    }

    [Test]
    public void GetValue ()
    {
      EnumValueInfo value = _enumerationReflector.GetValue (DomainAccessType.Journalize, _cache);

      Assert.IsNotNull (value);

      Assert.AreEqual (0, value.Value);
      Assert.AreEqual ("Journalize", value.Name);
      Assert.AreEqual ("00000002-0001-0000-0000-000000000000", value.ID);
    }

    [Test]
    public void GetValuesFromCache ()
    {
      Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (typeof (DomainAccessType), _cache);

      Assert.AreSame (values[DomainAccessType.Journalize], _cache.GetEnumValueInfo (DomainAccessType.Journalize));
      Assert.AreSame (values[DomainAccessType.Archive], _cache.GetEnumValueInfo (DomainAccessType.Archive));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The type 'System.String' is not an enumerated type.\r\nParameter name: type")]
    public void GetMetadataWithInvalidType ()
    {
      new EnumerationReflector ().GetValues (typeof (string), _cache);
    }
  }
}