using System;

namespace Rubicon.Mixins
{
  // Indicates that a mixin's method initialization parameter or a type parameter should be assigned the mixin's This value at initialization or
  // the mixin's This type at configuration analysis time.
  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  class ThisAttribute : Attribute
  {
  }

  // Indicates that a mixin's method initialization parameter or a type parameter should be assigned the mixin's Base value at initialization or
  // the mixin's Base type at configuration analysis time.
  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  class BaseAttribute : Attribute
  {
  }

  // Indicates that a mixin's method inizialization parameter should be assigned the mixin's Configuration value on initialization.
  [AttributeUsage (AttributeTargets.Parameter)]
  class ConfigurationAttribute : Attribute
  {
  }
}
