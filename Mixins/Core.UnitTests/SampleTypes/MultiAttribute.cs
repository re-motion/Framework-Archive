using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class MultiAttribute : Attribute { }
}