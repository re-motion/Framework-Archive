using System;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class NonInheritedAttribute : Attribute
  {
    public NonInheritedAttribute ()
    {
    }
  }
}