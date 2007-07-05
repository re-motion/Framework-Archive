using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  public class StubPropertyBase : PropertyBase
  {
    public StubPropertyBase (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (new Parameters(businessObjectProvider, propertyInfo, listInfo, isRequired))
    {
    }
  }
}