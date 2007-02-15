using System;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class PropertyValueMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PropertyValueMockEventReceiver (PropertyValue propertyValue)
    {
      ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

      propertyValue.Changed += new ValueChangeEventHandler (Changed);
      propertyValue.Changing += new ValueChangeEventHandler (Changing);
    }

    // abstract methods and properties

    public abstract void Changing (object sender, ValueChangeEventArgs args);
    public abstract void Changed (object sender, ValueChangeEventArgs args);

  }
}
