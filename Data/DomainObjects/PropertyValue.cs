using System;
using System.Collections;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// Represents a property of a domain object that is persisted by the framework.
/// </summary>
[Serializable]
public class PropertyValue
{
  // types

  // static members and constants

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
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ValueChangeEventHandler Changing;
  /// <summary>
  /// Occurs after the <see cref="Value"/> of the <see cref="PropertyValue"/> is changed.
  /// </summary>
  /// <include file='Doc\include\DomainObjects.xml' path='documentation/allEvents/remarks'/>
  public event ValueChangeEventHandler Changed;

  private PropertyDefinition _definition;
  private object _value;
  private object _originalValue;
  private bool _isDiscarded = false;
  private ArrayList _changeNotificationReceivers;

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/>.
  /// </summary>
  /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="definition"/> is <see langword="null"/>.</exception>
  /// <exception cref="InvalidEnumDefinitionException"><paramref name="definition"/> is a reference to an invalid enum.</exception>
  public PropertyValue (PropertyDefinition definition) : this (definition, definition.DefaultValue)
  {
  }

  /// <summary>
  /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/> and an initial <see cref="Value"/>.
  /// </summary>
  /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>. Must not be <see langword="null"/>.</param>
  /// <param name="value">The initial <see cref="Value"/> for the <b>PropertyValue</b>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="definition"/> is <see langword="null"/>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.InvalidTypeException"><paramref name="value"/> does not match the required type specified in <paramref name="definition"/>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.ValueTooLongException"><paramref name="value"/> is longer than the maximum length specified in <paramref name="definition"/>.</exception>
  public PropertyValue (PropertyDefinition definition, object value)
  {
    ArgumentUtility.CheckNotNull ("definition", definition);
    CheckValue (value, definition);

    _definition = definition;
    _value = value;
    _originalValue = value;
    _changeNotificationReceivers = new ArrayList ();
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
  /// <exception cref="Rubicon.Data.DomainObjects.InvalidTypeException"><paramref name="value"/> does not match the required type specified in <paramref name="definition"/>.</exception>
  /// <exception cref="Rubicon.Data.DomainObjects.ValueTooLongException"><paramref name="value"/> is longer than the maximum length specified in <paramref name="definition"/>.</exception>
  public object Value
  {
    get
    {
      CheckDiscarded ();

      // Note: A ClientTransaction extension could possibly raise an exception during BeginValueGet.
      //       If another ClientTransaction extension only wants to be notified on success it should use EndValueGet.
      BeginValueGet ();
      EndValueGet ();
      
      return _value;
    }
    set
    {
      CheckDiscarded ();
      CheckForRelationProperty ();

      if (AreValuesDifferent (_value, value))
      {
        CheckValue (value, _definition);
        BeginValueSet (value);

        object oldValue = _value;
        _value = value;

        EndValueSet (oldValue);
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

      // Note: A ClientTransaction extension could possibly raise an exception during BeginOriginalValueGet.
      //       If another ClientTransaction extension only wants to be notified on success it should use EndOriginalValueGet.
      BeginOriginalValueGet ();
      EndOriginalValueGet ();

      return _originalValue; 
    }
  }

  /// <summary>
  /// Indicates whether the <see cref="PropertyValue"/> may contain <see langword="null"/> as a value.
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
  /// <returns><see langword="true"/> if the specified <see cref="PropertyValue"/> is equal to the current <b>PropertyValue</b>; otherwise, <see langword="false"/>.</returns>
  /// <exception cref="DataManagement.ObjectDiscardedException">The object is already discarded. See <see cref="DataManagement.ObjectDiscardedException"/> for further information.</exception>
  public override bool Equals (object obj)
  {
    CheckDiscarded ();

    PropertyValue propertyValue = obj as PropertyValue;

    if (propertyValue != null)
    {
       return this._value.Equals (propertyValue._value)
          && this._originalValue.Equals (propertyValue._originalValue)
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
  /// Gets a value indicating the discarded status of the <see cref="PropertyValue"/>.
  /// </summary>
  /// <remarks>
  /// For more information why and when a <see cref="PropertyValue"/> is discarded see <see cref="Rubicon.Data.DomainObjects.DataManagement.ObjectDiscardedException"/>.
  /// </remarks>
  public bool IsDiscarded
  {
    get { return _isDiscarded; }
  }

  /// <summary>
  /// Raises the <see cref="Changing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ValueChangeEventArgs"/> object that contains the event data.</param>
  protected virtual void OnChanging (ValueChangeEventArgs args)
  {
    if (Changing != null)
      Changing (this, args);
  }

  /// <summary>
  /// Raises the <see cref="Changed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
  protected virtual void OnChanged (ValueChangeEventArgs args)
  {
    if (Changed != null)
      Changed (this, args);
  }

  internal void RegisterForChangeNotification (PropertyValueCollection propertyValueCollection)
  {
    ArgumentUtility.CheckNotNull ("propertyValueCollection", propertyValueCollection);
    _changeNotificationReceivers.Add (propertyValueCollection);
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

      if (value.GetType () == typeof (string))
        CheckStringValue ((string) value, definition);

      if (value.GetType () == typeof (byte[]))
        CheckByteArrayValue ((byte[]) value, definition);
    }
    else
    {
      if (!definition.IsNullable)
      {
        throw new InvalidOperationException (
            string.Format ("Property '{0}' does not allow null values.", definition.PropertyName));
      }
    }
  }

  private void CheckStringValue (string value, PropertyDefinition definition)
  {
    if (!definition.MaxLength.IsNull && value.Length > definition.MaxLength.Value)
    {
      string message = string.Format (
          "Value for property '{0}' is too long. Maximum number of characters: {1}.",
          definition.PropertyName, definition.MaxLength.Value);

      throw new ValueTooLongException (message, definition.PropertyName, definition.MaxLength.Value);
    }
  }

  private void CheckByteArrayValue (byte[] value, PropertyDefinition definition)
  {
    if (!definition.MaxLength.IsNull && value.Length > definition.MaxLength.Value)
    {
      string message = string.Format (
          "Value for property '{0}' is too large. Maximum size: {1}.", 
          definition.PropertyName, definition.MaxLength.Value);

      throw new ValueTooLongException (message, definition.PropertyName, definition.MaxLength.Value);
    }
  }

  private bool AreValuesDifferent (object value1, object value2)
  {
    if (value1 == null && value2 != null) return true;
    if (value1 != null && value2 == null) return true;
    if (value1 == null && value2 == null) return false;

    return !value1.Equals (value2);
  }

  private void BeginValueGet ()
  {
    foreach (PropertyValueCollection changeNotificationReceiver in _changeNotificationReceivers)
      changeNotificationReceiver.PropertyValue_Reading (this, _value, RetrievalType.CurrentValue);
  }

  private void BeginOriginalValueGet ()
  {
    foreach (PropertyValueCollection changeNotificationReceiver in _changeNotificationReceivers)
      changeNotificationReceiver.PropertyValue_Reading (this, _originalValue, RetrievalType.OriginalValue);
  }

  private void EndValueGet ()
  {
    foreach (PropertyValueCollection changeNotificationReceiver in _changeNotificationReceivers)
      changeNotificationReceiver.PropertyValue_Read (this, _value, RetrievalType.CurrentValue);
  }

  private void EndOriginalValueGet ()
  {
    foreach (PropertyValueCollection changeNotificationReceiver in _changeNotificationReceivers)
      changeNotificationReceiver.PropertyValue_Read (this, _originalValue, RetrievalType.OriginalValue);
  }

  private void BeginValueSet (object newValue)
  {
    ValueChangeEventArgs changingArgs = new ValueChangeEventArgs (_value, newValue);

    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of PropertyValueCollection when changing property values is not organized through events.
    foreach (PropertyValueCollection changeNotificationReceiver in _changeNotificationReceivers)
      changeNotificationReceiver.PropertyValue_Changing (this, changingArgs);

    OnChanging (changingArgs);
  }

  private void EndValueSet (object oldValue)
  {
    ValueChangeEventArgs changedArgs = new ValueChangeEventArgs (oldValue, _value);
    OnChanged (changedArgs);

    // Note: .NET 1.1 will not deserialize delegates to non-public (that means internal, protected, private) methods. 
    // Therefore notification of PropertyValueCollection when changing property values is not organized through events.
    foreach (PropertyValueCollection changeNotificationReceiver in _changeNotificationReceivers)
      changeNotificationReceiver.PropertyValue_Changed (this, changedArgs);
  }

  private void CheckForRelationProperty ()
  {
    if (_definition.PropertyType == typeof (ObjectID))
      throw new InvalidOperationException (string.Format ("The relation property '{0}' cannot be set directly.", _definition.PropertyName));
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
