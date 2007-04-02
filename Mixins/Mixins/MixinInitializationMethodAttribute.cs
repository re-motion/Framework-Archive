using System;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Method)]
  internal class MixinInitializationMethodAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  internal class ThisAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  internal class BaseAttribute : Attribute
  {
  }
}
