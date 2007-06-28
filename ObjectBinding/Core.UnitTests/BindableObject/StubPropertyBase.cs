using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  public class StubPropertyBase : PropertyBase
  {
    public StubPropertyBase (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
    }
  }
}