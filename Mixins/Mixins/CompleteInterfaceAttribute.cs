using System;

namespace Mixins
{
  [AttributeUsage (AttributeTargets.Interface)]
  public class CompleteInterfaceAttribute : Attribute
  {
    private Type _targetType;

    public CompleteInterfaceAttribute (Type targetType)
    {
      _targetType = targetType;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }
  }
}
