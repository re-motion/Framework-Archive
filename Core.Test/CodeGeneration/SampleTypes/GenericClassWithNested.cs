using System;

namespace Remotion.UnitTests.CodeGeneration.SampleTypes
{
  public class GenericClassWithNested<T>
  {
    public class Nested
    {
      public T T;
    }
  }
}