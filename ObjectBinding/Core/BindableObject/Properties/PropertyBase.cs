using System;
using System.Collections;
using System.Reflection;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject.Properties
{
  public abstract class PropertyBase : IBusinessObjectProperty
  {
    public struct Parameters
    {
      public readonly BindableObjectProvider BusinessObjectProvider;
      public readonly PropertyInfo PropertyInfo;
      public readonly Type UnderlyingType;
      public readonly IListInfo ListInfo;
      public readonly bool IsRequired;
      public readonly bool IsReadOnly;

      public Parameters (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, Type underlyingType, IListInfo listInfo,
          bool isRequired, bool isReadOnly)
      {
        ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
        ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

        BusinessObjectProvider = businessObjectProvider;
        PropertyInfo = propertyInfo;
        UnderlyingType = underlyingType;
        ListInfo = listInfo;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
      }
    }

    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly PropertyInfo _propertyInfo;
    private readonly IListInfo _listInfo;
    private readonly bool _isRequired;
    private readonly Type _underlyingType;
    private readonly bool _isReadOnly;
    private readonly bool _isNullable;

    protected PropertyBase (Parameters parameters)
    {
      _businessObjectProvider = parameters.BusinessObjectProvider;
      _propertyInfo = parameters.PropertyInfo;
      _underlyingType = parameters.UnderlyingType;
      _listInfo = parameters.ListInfo;
      _isRequired = parameters.IsRequired;
      _isReadOnly = parameters.IsReadOnly;
      _isNullable = GetNullability ();
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
          throw new InvalidOperationException (string.Format ("Cannot access ListInfo for non-list properties.\r\nProperty: {0}", Identifier));
        return _listInfo;
      }
    }

    /// <summary> Gets the type of the property. </summary>
    /// <remarks> 
    ///   <para>
    ///     This is the type of elements returned by the <see cref="IBusinessObject.GetProperty"/> method
    ///     and set via the <see cref="IBusinessObject.SetProperty"/> method.
    ///   </para><para>
    ///     If <see cref="IsList"/> is <see langword="true"/>, the property type must implement the <see cref="IList"/> 
    ///     interface, and the items contained in this list must have a type of <see cref="ListInfo"/>.<see cref="IListInfo.ItemType"/>.
    ///   </para>
    /// </remarks>
    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
    /// <value> A <see cref="String"/> by which this property can be found within its <see cref="IBusinessObjectClass"/>. </value>
    public string Identifier
    {
      get { return _propertyInfo.Name; }
    }

    /// <summary> Gets the property name as presented to the user. </summary>
    /// <value> The human readable identifier of this property. </value>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    public string DisplayName
    {
      get
      {
        IBindableObjectGlobalizationService globalizationService = BusinessObjectProvider.GetService<IBindableObjectGlobalizationService> ();
        if (globalizationService == null)
          return _propertyInfo.Name;
        return globalizationService.GetPropertyDisplayName (_propertyInfo);
      }
    }

    /// <summary> Gets a flag indicating whether this property is required. </summary>
    /// <value> <see langword="true"/> if this property is required. </value>
    /// <remarks> Setting required properties to <see langword="null"/> may result in an error. </remarks>
    public bool IsRequired
    {
      get { return _isRequired; }
    }

    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> of the <paramref name="obj"/>. </param>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can access this property. </returns>
    /// <remarks> The result may depend on the class, the user's authorization and/or the instance value. </remarks>
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

    /// <summary> Indicates whether this property can be modified by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can set this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
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

    /// <summary> Gets the <see cref="BindableObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="BindableObjectProvider"/> type. </value>
    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type. </value>
    IBusinessObjectProvider IBusinessObjectProperty.BusinessObjectProvider
    {
      get { return BusinessObjectProvider; }
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public virtual object ConvertFromNativePropertyType (object nativeValue)
    {
      return nativeValue;
    }

    public virtual object ConvertToNativePropertyType (object publicValue)
    {
      return publicValue;
    }

    protected Type UnderlyingType
    {
      get { return _underlyingType; }
    }

    protected bool IsNullable
    {
      get { return _isNullable; }
    }

    private bool GetNullability ()
    {
      return Nullable.GetUnderlyingType (IsList ? ListInfo.ItemType : PropertyType) != null;
    }
  }
}
