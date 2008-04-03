using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class MultiAttribute : Attribute { }
}