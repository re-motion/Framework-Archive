using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithoutIDProperty : TestDomainBase
{
  // types

  // static members and constants

  public static new ClassWithoutIDProperty GetObject (ObjectID id)
  {
    return (ClassWithoutIDProperty) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithoutIDProperty ()
  {
  }

  protected ClassWithoutIDProperty (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

}
}
