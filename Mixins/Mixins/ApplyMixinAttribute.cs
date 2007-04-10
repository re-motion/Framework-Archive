using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Utilities;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)] // ApplyMixinAttribute is inherited
  public class ApplyMixinAttribute : Attribute
  {
    private Type _mixinType;

    public ApplyMixinAttribute (Type mixinType)
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
