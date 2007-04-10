using System;
using Rubicon.Utilities;

namespace Mixins
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)] // MixinForAttribute is not inherited!
  public class MixinForAttribute : Attribute
  {
    private Type _targetType;

    public MixinForAttribute (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      _targetType = targetType;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }
  }
}
