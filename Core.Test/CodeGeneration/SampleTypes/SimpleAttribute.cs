using System;

namespace Remotion.Core.UnitTests.CodeGeneration.SampleTypes
{
  [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = true)]
  public class SimpleAttribute : Attribute
  {
    public string S;
  }
}