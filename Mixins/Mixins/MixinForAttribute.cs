using System;

namespace Mixins
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)] // MixinForAttribute is not inherited!
  public class MixinForAttribute : Attribute
  {
    private Type _targetType;

    public MixinForAttribute (Type targetType)
    {
      _targetType = targetType;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }
  }
}
