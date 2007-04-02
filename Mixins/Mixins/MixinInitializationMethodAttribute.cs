using System;

namespace Mixins
{
  [AttributeUsage(AttributeTargets.Method)]
  internal class MixinInitializationMethodAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter)]
  internal class ThisAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter)]
  internal class BaseAttribute : Attribute
  {
  }
}
