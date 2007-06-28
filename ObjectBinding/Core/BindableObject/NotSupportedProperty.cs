using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  public sealed class NotSupportedProperty : PropertyBase
  {
    public NotSupportedProperty (PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (propertyInfo, listInfo, isRequired)
    {
    }
  }
}