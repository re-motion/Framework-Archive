using System;

namespace Remotion.Core.UnitTests.CodeGeneration.SampleTypes
{
  public class GenericClassWithNested<T>
  {
    public class Nested
    {
      public T T;
    }
  }
}