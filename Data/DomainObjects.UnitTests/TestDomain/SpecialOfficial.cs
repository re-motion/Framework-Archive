using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  public abstract class SpecialOfficial : Official
  {
    public static new SpecialOfficial NewObject ()
    {
      return DomainObject.NewObject<SpecialOfficial>().With();
    }

    protected SpecialOfficial()
    {
    }

    protected SpecialOfficial (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}
