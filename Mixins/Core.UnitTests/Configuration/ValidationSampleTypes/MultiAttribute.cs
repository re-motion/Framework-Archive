using System;

namespace Mixins.UnitTests.Configuration.ValidationSampleTypes
{
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class MultiAttribute : Attribute { }
}