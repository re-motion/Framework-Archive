using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithoutTimestampProperty : TestDomainBase
{
  // types

  // static members and constants

  public static new ClassWithoutTimestampProperty GetObject (ObjectID id)
  {
    return (ClassWithoutTimestampProperty) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithoutTimestampProperty ()
  {
  }

  protected ClassWithoutTimestampProperty (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

}
}
