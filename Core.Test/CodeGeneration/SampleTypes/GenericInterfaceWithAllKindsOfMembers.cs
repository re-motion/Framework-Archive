using System;

namespace Remotion.UnitTests.CodeGeneration.SampleTypes
{
  public interface GenericInterfaceWithAllKindsOfMembers<T>
  {
    string Method (T t);
    T Property { get; }
    event Func<T> Event;
  }
}