using System;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects
{
public class PropertyValue
{
  // types

  // static members and constants

  private static object GetDefaultValue (PropertyDefinition propertyDefinition)
  {
    switch (propertyDefinition.PropertyType.FullName)
    {
      case "Rubicon.Data.DomainObjects.ObjectID":
        return null;

      case "System.Boolean":
        return false;

      case "System.Byte":
        return (byte) 0;

      case "System.DateTime":
        return DateTime.MinValue;

      case "System.Decimal":
        return (decimal) 0;

      case "System.Double":
        return (double) 0.0;

      case "System.Guid":
        return Guid.Empty;

      case "System.Int16":
        return (short) 0;

      case "System.Int32":
        return (int) 0;

      case "System.Int64":
        return (long) 0;

      case "System.Single":
        return (float) 0.0;

      case "System.String":
        if (propertyDefinition.IsNullable)
          return null;
        else
          return string.Empty;

      case "System.Char":
        return ' ';

      case "Rubicon.NullableValueTypes.NaBoolean":
        return NaBoolean.Null;

      case "Rubicon.NullableValueTypes.NaDateTime":
        return NaDateTime.Null;

      case "Rubicon.NullableValueTypes.NaDouble":
        return NaDouble.Null;

      case "Rubicon.NullableValueTypes.NaInt32":
        return NaInt32.Null;

      default:
        if (propertyDefinition.PropertyType.IsEnum)
          return GetDefaultEnumValue (propertyDefinition);

        throw new UnknownPropertyTypeException (propertyDefinition.PropertyType);
    }
  }

  private static object GetDefaultEnumValue (PropertyDefinition propertyDefinition)
  {
    Array enumValues = Enum.GetValues (propertyDefinition.PropertyType);

    if (enumValues.Length > 0)
      return enumValues.GetValue (0);
  
    throw new InvalidEnumDefinitionException (propertyDefinition.PropertyType);
  }

  // member fields

  public event ValueChangingEventHandler Changing;
  public event EventHandler Changed;

  private PropertyDefinition _definition;
  private object _value;
  private object _originalValue;
  private bool _isDiscarded = false;

  // construction and disposing

  public PropertyValue (PropertyDefinition definition) : this (definition, GetDefaultValue (definition))
  {
  }

  public PropertyValue (PropertyDefinition definition, object value)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    CheckValue (value, definition);

    _definition = definition;
    _value = value;
    _originalValue = value;
  }

  // methods and properties

  public PropertyDefinition Definition
  {
    get 
    {
      CheckDiscarded ();
      return _definition; 
    }
  }

  public string Name
  {
    get 
    {
      CheckDiscarded ();
      return _definition.PropertyName; 
    }
  }

  public Type PropertyType
  {
    get 
    {
      CheckDiscarded ();
      return _definition.PropertyType; 
    }
  }

  public object Value
  {
    get
    {
      CheckDiscarded ();
      return _value;
    }
    set
    {
      CheckDiscarded ();

      if (AreValuesDifferent (_value, value))
      {
        CheckValue (value, _definition);

        ValueChangingEventArgs changingArgs = new ValueChangingEventArgs (_value, value);
        OnChanging (changingArgs);

        if (!changingArgs.Cancel)
        {
          _value = value;
          OnChanged (new EventArgs ());
        }
      }
    }
  }

  public object OriginalValue
  {
    get 
    { 
      CheckDiscarded ();
      return _originalValue; 
    }
  }

  public bool IsNullable
  {
    get 
    {
      CheckDiscarded ();
      return _definition.IsNullable; 
    }
  }

  public NaInt32 MaxLength 
  {
    get 
    {
      CheckDiscarded ();
      return _definition.MaxLength; 
    }
  }

  public bool HasChanged
  {
    get 
    {
      CheckDiscarded ();
      return AreValuesDifferent (_value, _originalValue); 
    }
  }

  public override bool Equals (object obj)
  {
    CheckDiscarded ();

    PropertyValue propertyValue = obj as PropertyValue;

    if (propertyValue != null)
    {
       return this.Value.Equals (propertyValue.Value)
          && this.OriginalValue.Equals (propertyValue.OriginalValue)
          && this.HasChanged.Equals (propertyValue.HasChanged)
          && this.Definition.Equals (propertyValue.Definition);
    }
    else
    {
      return false;
    }
  }

  public override int GetHashCode()
  {
    CheckDiscarded ();
    return _definition.PropertyName.GetHashCode () ^ _value.GetHashCode () ^ _originalValue.GetHashCode ();
  }

  public void Discard ()
  {
    _isDiscarded = true;
  }

  protected virtual void OnChanging (ValueChangingEventArgs args)
  {
    if (Changing != null)
      Changing (this, args);
  }

  protected virtual void OnChanged (EventArgs args)
  {
    if (Changed != null)
      Changed (this, args);
  }

  internal void Commit ()
  {
    if (HasChanged)
      _originalValue = _value;
  }

  internal void Rollback ()
  {
    if (HasChanged)
      _value = _originalValue;
  }

  internal void SetRelationValue (ObjectID id)
  {
    if (AreValuesDifferent (_value, id))
    {
      CheckValue (id, _definition);
      _value = id;
    }
  }

  private void CheckValue (object value, PropertyDefinition definition)
  {
    if (value != null)
    {
      if (definition.PropertyType != typeof (object))
      {
        if (value.GetType () != definition.PropertyType)
          throw new InvalidTypeException (definition.PropertyName, definition.PropertyType, value.GetType ());
      }

      if (value.GetType () == typeof (string) && !definition.MaxLength.IsNull)
      {
        string stringValue = (string) value;
        if (stringValue.Length > definition.MaxLength.Value)
          throw new ValueTooLongException (definition.PropertyName, definition.MaxLength.Value);
      }
    }
    else
    {
      if (definition.PropertyType == typeof (string) && !definition.IsNullable)
      {
        throw new InvalidOperationException (
            string.Format ("Property '{0}' does not allow null values.", definition.PropertyName));
      }
    }
  }

  private bool AreValuesDifferent (object value1, object value2)
  {
    if (value1 == null && value2 != null) return true;
    if (value1 != null && value2 == null) return true;
    if (value1 == null && value2 == null) return false;

    return !value1.Equals (value2);
  }

  private ArgumentException CreateArgumentException (string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args));
  }

  private void CheckDiscarded ()
  {
    if (_isDiscarded)
      throw new ObjectDiscardedException ();
  }
}
}
