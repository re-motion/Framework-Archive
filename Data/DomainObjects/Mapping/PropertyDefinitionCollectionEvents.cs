using System;
using System.ComponentModel;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Mapping
{

public delegate void PropertyDefinitionAddingEventHandler (object sender, PropertyDefinitionAddingEventArgs args);
public delegate void PropertyDefinitionAddedEventHandler (object sender, PropertyDefinitionAddedEventArgs args);

public class PropertyDefinitionAddingEventArgs : CancelEventArgs
{
  private PropertyDefinition _propertyDefinition;

  public PropertyDefinitionAddingEventArgs (bool cancel, PropertyDefinition propertyDefinition) : base (cancel)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    _propertyDefinition = propertyDefinition;
  }

  public PropertyDefinitionAddingEventArgs (PropertyDefinition propertyDefinition) : this (false, propertyDefinition)
  {
  }

  public PropertyDefinition PropertyDefinition
  {
    get { return _propertyDefinition; }
  }
}

public class PropertyDefinitionAddedEventArgs : EventArgs
{
  private PropertyDefinition _propertyDefinition;

  public PropertyDefinitionAddedEventArgs (PropertyDefinition propertyDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

    _propertyDefinition = propertyDefinition;
  }

  public PropertyDefinition PropertyDefinition
  {
    get { return _propertyDefinition; }
  }
}
}
