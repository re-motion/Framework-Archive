using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithoutClassIDProperty : TestDomainBase
{
  // types

  // static members and constants

  public static new ClassWithoutClassIDProperty GetObject (ObjectID id)
  {
    return (ClassWithoutClassIDProperty) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithoutClassIDProperty ()
  {
  }

  protected ClassWithoutClassIDProperty (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

}
}
