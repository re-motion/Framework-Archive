using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class DateProperty : DateTimePropertyBase
  {
    public DateProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.Date; }
    }
  }
}