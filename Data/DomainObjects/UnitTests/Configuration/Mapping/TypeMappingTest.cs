using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class TypeMappingTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public TypeMappingTest ()
  {
  }

  // methods and properties

  [Test]
  public void TypeMapping ()
  {
    string[] mappingTypes = new string[]
    {
      "byte",
      "boolean",
      "date",
      "dateTime", 
      "single",
      "decimal", 
      "double", 
      "guid", 
      "int16", 
      "int32", 
      "int64", 
      "string", 
      "char", 
      "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.UnitTests",
      "objectID"
    };

    Type[] expectedTypes = new Type[]
    {
      typeof (byte),
      typeof (bool),
      typeof (DateTime),
      typeof (DateTime),
      typeof (float),
      typeof (decimal),
      typeof (double),
      typeof (Guid),
      typeof (short),
      typeof (int),
      typeof (long),
      typeof (string),
      typeof (char),
      typeof (Customer.CustomerType),
      typeof (ObjectID)
    };

    for (int i = 0; i < mappingTypes.Length; i++)
    {
      Assert.AreEqual (expectedTypes[i], MappingUtility.MapType (mappingTypes[i]));
    }
  }
}
}
