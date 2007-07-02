using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class ByteProperty : NumericPropertyBase
  {
    public ByteProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    public override bool AllowNegative
    {
      get { return false; }
    }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    public override Type Type
    {
      get { return typeof (Byte); }
    }
  }
}