using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents a property of a domain object that is persisted by the framework.
/// </summary>
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

      // TODO: Move this code to MappingUtility. Review when adding new Na* types.
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

  /// <summary>
  /// Occurs before the <see cref="Value"/> of the <see cref="PropertyValue"/> is changed.
  /// </summary>
  public event ValueChangingEventHandler Changing;
  /// <summary>
  /// Occurs after the <see cref="Value"/> of the <see cref="PropertyValue"/> is changed.
  /// </summary>
  public event EventHandler Changed;

  private PropertyDefinition _definition;
  private object _value;
  private object _originalValue;
  private bool _isDiscarded = false;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/>.
  /// </summary>
  /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>.</param>
  /// <exception cref="System.ArgumentNullException"><i>definition</i> is a null reference.</exception>
  /// <exception cref="InvalidEnumDefinitionException"><i>definition</i> is a reference to an invalid enum.</exception>
  public PropertyValue (PropertyDefinition definition) : this (definition, GetDefaultValue (definition))
  {
  }

  /// <summary>
  /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/> and an initial <see cref="Value"/>.
  /// </summary>
  /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>.</param>
  /// <param name="value">The initial <see cref="Value"/> for the <b>PropertyValue</b>.</param>
  /// <exception cref="System.ArgumentNullException"><i>definition</i> is a null reference.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.InvalidTypeException"><i>value</i> does not match the required type specified in <i>definition</i>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.ValueTooLongException"><i>value</i> is longer than the maximum length specified in <i>definition</i>.</exception>
  public PropertyValue (PropertyDefinition definition, object value)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    CheckValue (value, definition);

    _definition = definition;
    _value = value;
    _originalValue = value;
  }

  // methods and properties

  /// <summary>
  /// Gets the <see cref="PropertyDefinition"/> of the <see cref="PropertyValue"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public PropertyDefinition Definition
  {
    get 
    {
      CheckDiscarded ();
      return _definition; 
    }
  }

  /// <summary>
  /// Gets the name of the <see cref="PropertyValue"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public string Name
  {
    get 
    {
      CheckDiscarded ();
      return _definition.PropertyName; 
    }
  }

  /// <summary>
  /// Gets the <see cref="Type"/> of the <see cref="Value"/> of a <see cref="PropertyValue"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public Type PropertyType
  {
    get 
    {
      CheckDiscarded ();
      return _definition.PropertyType; 
    }
  }

  /// <summary>
  /// Gets or sets the value of the <see cref="PropertyValue"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.InvalidTypeException"><i>value</i> does not match the required type specified in <i>definition</i>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.ValueTooLongException"><i>value</i> is longer than the maximum length specified in <i>definition</i>.</exception>
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

        _value = value;
        OnChanged (new EventArgs ());
      }
    }
  }

  /// <summary>
  /// Gets the original <see cref="Value"/> of the <see cref="PropertyValue"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public object OriginalValue
  {
    get 
    { 
      CheckDiscarded ();
      return _originalValue; 
    }
  }

  /// <summary>
  /// Indicates whether the <see cref="PropertyValue"/> may contain null as a value.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool IsNullable
  {
    get 
    {
      CheckDiscarded ();
      return _definition.IsNullable; 
    }
  }

  /// <summary>
  /// Gets the maximum length of the <see cref="Value"/> of the <see cref="PropertyValue"/>.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public NaInt32 MaxLength 
  {
    get 
    {
      CheckDiscarded ();
      return _definition.MaxLength; 
    }
  }

  /// <summary>
  /// Indicates if the <see cref="Value"/> of the <see cref="PropertyValue"/> has changed since instantiation, loading, commit or rollback.
  /// </summary>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public bool HasChanged
  {
    get 
    {
      CheckDiscarded ();
      return AreValuesDifferent (_value, _originalValue); 
    }
  }


  /// <summary>
  /// Determines whether the specified <see cref="PropertyValue"/> is equal to the current <b>PropertyValue</b>.
  /// </summary>
  /// <param name="obj">The <see cref="PropertyValue"/> to compare with the current <b>PropertyValue</b>. </param>
  /// <returns><b>true</b> if the specified <see cref="PropertyValue"/> is equal to the current <b>PropertyValue</b>; otherwise, <b>false</b>.</returns>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
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

  /// <summary>
  /// Returns the hash code for this instance.
  /// </summary>
  /// <returns>A 32-bit signed integer hash code.</returns>
  public override int GetHashCode()
  {
    CheckDiscarded ();
    return _definition.PropertyName.GetHashCode () ^ _value.GetHashCode () ^ _originalValue.GetHashCode ();
  }

  /// <summary>
  /// Raises the <see cref="Changing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ValueChangingEventArgs"/> object that contains the event data.</param>
  protected virtual void OnChanging (ValueChangingEventArgs args)
  {
    if (Changing != null)
      Changing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Changed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
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

  internal void Discard ()
  {
    _isDiscarded = true;
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
