using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
public class PropertyValueChecker
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PropertyValueChecker ()
  {
  }

  // methods and properties

  public void Check (PropertyValue expectedValue, PropertyValue actualValue)
  {
    Assert.IsNotNull (actualValue, expectedValue.Name);

    Assert.AreEqual (expectedValue.Name, actualValue.Name, "Name");
    
    Assert.AreEqual (expectedValue.Value, actualValue.Value, 
        string.Format ("Value, expected property name: '{0}'", expectedValue.Name));

    Assert.AreEqual (expectedValue.OriginalValue, actualValue.OriginalValue, 
        string.Format ("OriginalValue, expected property name: '{0}'", expectedValue.Name));

    Assert.AreEqual (expectedValue.HasChanged, actualValue.HasChanged, 
        string.Format ("HasChanged, expected property name: '{0}'", expectedValue.Name));
  }
}
}
