using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class SpecialOfficial : Official
{
  // types

  // static members and constants

  public static new SpecialOfficial GetObject (ObjectID id)
  {
    return (SpecialOfficial) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public SpecialOfficial ()
  {
  }

  public SpecialOfficial (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected SpecialOfficial (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

}
}
