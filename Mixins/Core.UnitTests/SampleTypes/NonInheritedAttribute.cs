using System;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class NonInheritedAttribute : Attribute
  {
    public NonInheritedAttribute ()
    {
    }
  }
}