using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public abstract class NumericPropertyBase : PropertyBase, IBusinessObjectNumericProperty
  {
    protected NumericPropertyBase (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    public abstract Type Type { get; }

    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    public abstract bool AllowNegative { get; }
  }
}