using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public abstract class DateTimePropertyBase : PropertyBase, IBusinessObjectDateTimeProperty
  {
    protected DateTimePropertyBase (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    public abstract DateTimeType Type { get; }
  }
}