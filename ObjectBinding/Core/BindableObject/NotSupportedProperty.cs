using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  public sealed class NotSupportedProperty : PropertyBase
  {
    public NotSupportedProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }
  }
}