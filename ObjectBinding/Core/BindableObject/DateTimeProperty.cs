using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class DateTimeProperty : DateTimePropertyBase
  {
    public DateTimeProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.DateTime; }
    }
  }
}