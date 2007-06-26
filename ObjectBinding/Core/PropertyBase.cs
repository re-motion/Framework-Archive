using System;
using System.Collections;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  public abstract class PropertyBase : IBusinessObjectProperty
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly bool _isList;
    private readonly Type _itemType;
    private readonly bool _isRequired;


    protected PropertyBase (PropertyInfo propertyInfo, Type itemType, bool isList, bool isRequired)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("itemType", itemType);

      _propertyInfo = propertyInfo;
      _itemType = itemType;
      _isList = isList;
      _isRequired = isRequired;
    }

    /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
    /// <value> <see langword="true"/> if this property contains multiple values. </value>
    /// <remarks> Multiple values are provided via any type implementing <see cref="IList"/>. </remarks>
    public bool IsList
    {
      get { return _isList; }
    }

    /// <summary> Creates a list. </summary>
    /// <returns> A new list with the specified number of empty elements. </returns>
    /// <remarks>
    ///   Use this method to create a new list in order to ensure that the correct list type is used
    ///   (<see cref="Array"/>, <see cref="ArrayList"/>, etc.)
    /// </remarks>
    public IList CreateList (int count)
    {
      throw new NotImplementedException();
    }

    /// <summary> Gets the type of a single value item. </summary>
    /// <remarks> If <see cref="IsList"/> is <see langword="false"/>, the item type is the same as 
    ///   <see cref="PropertyType"/>. 
    ///   Otherwise, the item type is the type of a list item.
    /// </remarks>
    public Type ItemType
    {
      get { return _itemType; }
    }

    /// <summary> Gets the type of the property. </summary>
    /// <remarks> 
    ///   <para>
    ///     This is the type of elements returned by the <see cref="IBusinessObject.GetProperty"/> method
    ///     and set via the <see cref="IBusinessObject.SetProperty"/> method.
    ///   </para><para>
    ///     If <see cref="IsList"/> is <see langword="true"/>, the property type must implement the <see cref="IList"/> 
    ///     interface, and the items contained in this list must have a type of <see cref="ItemType"/>.
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
      get { throw new NotImplementedException(); }
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
      throw new NotImplementedException();
    }

    /// <summary> Indicates whether this property can be modified by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can set this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
    public bool IsReadOnly (IBusinessObject obj)
    {
      throw new NotImplementedException();
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type. </value>
    /// <remarks>
    ///   <note type="inotes">
    ///     Must not return <see langword="null"/>.
    ///   </note>
    /// </remarks>
    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { throw new NotImplementedException(); }
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }
  }
}