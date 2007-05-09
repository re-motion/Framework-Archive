using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)] // UsesMixinAttribute is inherited
  public class UsesAttribute : Attribute
  {
    private Type _mixinType;

    public UsesAttribute (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      _mixinType = mixinType;
    }

    public Type MixinType
    {
      get { return _mixinType; }
    }
  }
}
