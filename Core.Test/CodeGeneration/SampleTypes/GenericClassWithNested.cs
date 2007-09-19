using System;

namespace Rubicon.Core.UnitTests.CodeGeneration.SampleTypes
{
  public class GenericClassWithNested<T>
  {
    public class Nested
    {
      public T T;
    }
  }
}