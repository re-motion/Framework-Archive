using System;
using Rubicon.Utilities;

namespace Mixins
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true, Inherited = false)] // ExtendsAttribute is not inherited!
  public class ExtendsAttribute : Attribute
  {
    private Type _targetType;

    public ExtendsAttribute (Type targetType)
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
