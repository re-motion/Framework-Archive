using System;

namespace Mixins
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)] // MixinAttribute is not inherited!
  public class MixinAttribute : Attribute
  {
    private Type _targetType;

    public MixinAttribute (Type targetType)
    {
      _targetType = targetType;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }
  }
}
