using System;

namespace Remotion.UnitTests.CodeGeneration.SampleTypes
{
  [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = true)]
  public class SimpleAttribute : Attribute
  {
    public string S;
  }
}