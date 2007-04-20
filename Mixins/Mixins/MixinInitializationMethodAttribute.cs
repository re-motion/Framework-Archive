using System;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Method)]
  public class MixinInitializationMethodAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  public class ThisAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  public class BaseAttribute : Attribute
  {
  }
}
