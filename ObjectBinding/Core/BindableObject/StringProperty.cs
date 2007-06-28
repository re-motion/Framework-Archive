using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class StringProperty : PropertyBase, IBusinessObjectStringProperty
  {
    private readonly int? _maxLength;

    public StringProperty (PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired, int? maxLength)
        : base (propertyInfo, listInfo, isRequired)
    {
      _maxLength = maxLength;
    }

    /// <summary>
    ///   Getsthe the maximum length of a string assigned to the property, or <see langword="null"/> if no maximum length is defined.
    /// </summary>
    public int? MaxLength
    {
      get { return _maxLength; }
    }
  }
}