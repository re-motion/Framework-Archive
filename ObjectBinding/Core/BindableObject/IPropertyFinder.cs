using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject.Properties;

namespace Rubicon.ObjectBinding.BindableObject
{
  public interface IPropertyFinder
  {
    IEnumerable<IPropertyInformation> GetPropertyInfos ();
  }
}