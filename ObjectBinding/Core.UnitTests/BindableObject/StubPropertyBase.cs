using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  public class StubPropertyBase : PropertyBase
  {
    public StubPropertyBase (PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (propertyInfo, listInfo, isRequired)
    {
    }
  }
}