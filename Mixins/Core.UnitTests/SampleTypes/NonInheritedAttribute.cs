using System;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class NonInheritedAttribute : Attribute
  {
    public NonInheritedAttribute ()
    {
    }
  }
}