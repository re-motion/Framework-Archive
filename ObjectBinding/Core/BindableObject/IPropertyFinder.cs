using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  public interface IPropertyFinder
  {
    IEnumerable<PropertyInfo> GetPropertyInfos ();
  }
}