using System;

namespace Rubicon.Core.UnitTests.CodeGeneration.SampleTypes
{
  public interface GenericInterfaceWithAllKindsOfMembers<T>
  {
    string Method (T t);
    T Property { get; }
    event Func<T> Event;
  }
}