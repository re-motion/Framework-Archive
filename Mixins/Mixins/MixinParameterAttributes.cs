using System;

namespace Mixins
{
  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  public class ThisAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
  public class BaseAttribute : Attribute
  {
  }

  [AttributeUsage (AttributeTargets.Parameter)]
  public class ConfigurationAttribute : Attribute
  {
  }
}
