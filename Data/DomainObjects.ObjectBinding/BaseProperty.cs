using System;
using System.Collections;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Globalization;
using Rubicon.ObjectBinding;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The default implementation of <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> for the DomainObjects framework.
  /// </summary>
  /// <remarks>See the documentation of <see cref="Rubicon.ObjectBinding.IBusinessObjectProperty"/> for additional documentation for each member.</remarks>
  public class BaseProperty: IBusinessObjectProperty
  {
    private PropertyInfo _propertyInfo;
    private bool _isRequired;
    private bool _isReadOnly;
    private Type _underlyingType;
    private IListInfo _listInfo;
    private IBusinessObjectClass _businessObjectClass;

    /// <summary>
    /// Instantiates a new object.
    /// </summary>
    /// <param name="businessObjectClass">The <see cref="IBusinessObjectClass"/> to which this <see cref="IBusinessObjectProperty"/> belongs.</param>
    /// <param name="propertyInfo">The <see cref="System.Reflection.PropertyInfo"/> object of the property.</param>
    /// <param name="isRequired">A value indicating if the property is required by the business model, or not.</param>
    /// <param name="underlyingType">The type of the property value.</param>
    /// <param name="isList">A value indicating if the property contains multiple objects.</param>
    protected internal BaseProperty (IBusinessObjectClass businessObjectClass, PropertyInfo propertyInfo, bool isRequired, Type underlyingType, bool isList)
    {
      ArgumentUtility.CheckNotNull ("businessObjectClass", businessObjectClass);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      _businessObjectClass = businessObjectClass;
      _propertyInfo = propertyInfo;
      _isRequired = isRequired;
      _underlyingType = underlyingType;
      _listInfo = isList ? new ListInfo (underlyingType) : null;

      //TODO: Remove this code once the Data.DomainObjects.ObjectBinding.Legacy assembly has been extracted.
      if (propertyInfo.CanWrite || (IsList && typeof (IList).IsAssignableFrom (propertyInfo.PropertyType)))
      {
        IsReadOnlyAttribute isReadOnlyAttribute = AttributeUtility.GetCustomAttribute<IsReadOnlyAttribute> (propertyInfo, true);
        _isReadOnly = isReadOnlyAttribute != null;
      }
      else
      {
        _isReadOnly = true;
      }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _businessObjectClass; }
    }

    /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
    /// <value> <see langword="true"/> if this property contains multiple values. </value>
    /// <remarks> Multiple values are provided via any type implementing <see cref="IList"/>. </remarks>
    public bool IsList
    {
      get { return _listInfo != null; }
    }

    /// <summary>Gets the <see cref="IListInfo"/> for this <see cref="IBusinessObjectProperty"/>.</summary>
    /// <value>An onject implementing <see cref="IListInfo"/>.</value>
    /// <exception cref="InvalidOperationException">Thrown if the property is not a list property.</exception>
    public IListInfo ListInfo
    {
      get
      {
        if (_listInfo == null)
          throw new InvalidOperationException ("Cannot access ListInfo for non-list properties.");
        return _listInfo;
      }
    }

    /// <summary> Gets the type of a single value item. </summary>
    /// <remarks>If <see cref="IsList"/> is <see langword="false"/>, the item type is the same as <see cref="PropertyType"/>. 
    ///   Otherwise, the item type is the type of a list item.
    /// </remarks>
    public Type UnderlyingType
    {
      get { return _underlyingType; }
    }

    /// <summary> Gets the type of the property. </summary>
    /// <remarks> 
    ///   This is the type of elements returned by the <see cref="Rubicon.ObjectBinding.IBusinessObject.GetProperty"/> method
    ///   and set via the <see cref="Rubicon.ObjectBinding.IBusinessObject.SetProperty"/> method.
    /// </remarks>
    public virtual Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
    /// <value>The name of the property.</value>
    public string Identifier
    {
      get { return _propertyInfo.Name; }
    }

    /// <summary> Gets the property name as presented to the user. </summary>
    /// <value>The human readable identifier of this property in the current culture.</value>
    public string DisplayName
    {
      get
      {
        string displayName = string.Empty;

        if (MultiLingualResourcesAttribute.ExistsResource (_propertyInfo.DeclaringType))
          displayName = MultiLingualResourcesAttribute.GetResourceText (_propertyInfo.DeclaringType, "property:" + _propertyInfo.Name);

        if (displayName == string.Empty || displayName == null)
          displayName = _propertyInfo.Name;

        return displayName;
      }
    }

    /// <summary> Gets a flag indicating whether this property is required. </summary>
    /// <value><see langword="true"/> if this property is required; otherwise <see langword="false"/>. </value>
    public virtual bool IsRequired
    {
      get { return _isRequired; }
    }

    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="objectClass"> 
    ///   The <see cref="IBusinessObjectClass"/> of the <paramref name="obj"/>. 
    ///   This parameter is not used in the current implementation.
    /// </param>
    /// <param name="obj">
    ///   The object to evaluate this property for, or <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/></returns>
    public bool IsAccessible (IBusinessObjectClass objectClass, IBusinessObject obj)
    {
      ISecurableObject securableObject = obj as ISecurableObject;
      if (securableObject == null)
        return true;

      IObjectSecurityAdapter objectSecurityAdapter = SecurityAdapterRegistry.Instance.GetAdapter<IObjectSecurityAdapter>();
      if (objectSecurityAdapter == null)
        return true;

      return objectSecurityAdapter.HasAccessOnGetAccessor (securableObject, _propertyInfo.Name);
    }

    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="obj">
    ///   The object to evaluate this property for, or <see langword="null"/>.
    /// </param>
    /// <returns><see langword="true"/></returns>
    public bool IsAccessible (IBusinessObject obj)
    {
      return IsAccessible (null, obj);
    }

    /// <summary>Indicates whether this property can be modified by the user.</summary>
    /// <returns><see langword="true"/> if the user can set this property; otherwise <see langword="false"/>.</returns>
    public bool IsReadOnly (IBusinessObject obj)
    {
      if (_isReadOnly)
        return true;

      ISecurableObject securableObject = obj as ISecurableObject;
      if (securableObject == null)
        return false;

      IObjectSecurityAdapter objectSecurityAdapter = SecurityAdapterRegistry.Instance.GetAdapter<IObjectSecurityAdapter>();
      if (objectSecurityAdapter == null)
        return false;

      return !objectSecurityAdapter.HasAccessOnSetAccessor (securableObject, _propertyInfo.Name);
    }

    /// <summary>
    /// Gets the <see cref="System.Reflection.PropertyInfo"/> object representing the property.
    /// </summary>
    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    /// <summary>
    /// Converts the value from internal representation (business layer) to external representation (representation layer).
    /// </summary>
    /// <param name="bindableObject">The <see cref="IBusinessObject"/> from which the <paramref name="internalValue"/> has been read.</param>
    /// <param name="internalValue">The internal value.</param>
    /// <returns>The external value. If the data type of the property has a field named "MinValue" and <paramref name="internalValue"/> is equal to it, then the return value is null.</returns>
    /// <note type="inheritinfo">
    ///   Override this method to perform conversions of data types or other logic.
    /// </note>
    public virtual object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      ArgumentUtility.CheckNotNull ("bindableObject", bindableObject);

      if (!IsList && IsDefaultValue (bindableObject, internalValue))
        return null;

      return internalValue;
    }

    /// <summary>
    /// Converts the value from external representation (representation layer) to internal representation.
    /// </summary>
    /// <param name="publicValue">The external value.</param>
    /// <returns>The internal value.</returns>
    /// <exception cref="InvalidNullAssignmentException"><paramref name="publicValue"/> is <see langword="null"/> and this value is not supported by this property.</exception>
    public virtual object ToInternalType (object publicValue)
    {
      if (_underlyingType.IsValueType && publicValue == null)
        throw new InvalidNullAssignmentException (_underlyingType);

      return publicValue;
    }

    /// <summary>
    /// Gets an IBusinessObjectProvider for the domain model.
    /// </summary>
    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { return DomainObjectProvider.Instance; }
    }

    protected bool IsDefaultValue (IBusinessObject bindableObject, object internalValue)
    {
      DomainObjectClass domainObjectClass = _businessObjectClass as DomainObjectClass;
      if (domainObjectClass != null && domainObjectClass.ClassDefinition is ReflectionBasedClassDefinition)
      {
        BindableDomainObject bindableDomainObject = (BindableDomainObject) bindableObject;
        return bindableDomainObject.HasPropertyStillDefaultValue (ReflectionUtility.GetPropertyName (_propertyInfo)) ?? false;
      }
      else
      {
        FieldInfo minValueFieldInfo = _underlyingType.GetField ("MinValue");

        if (minValueFieldInfo == null)
          return false;

        return minValueFieldInfo.GetValue (_underlyingType).ToString () == internalValue.ToString ();
      }
    }
  }
}