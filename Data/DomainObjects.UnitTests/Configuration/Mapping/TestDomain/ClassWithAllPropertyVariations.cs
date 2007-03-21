using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain
{
  [Serializable]
  public class ClassWithAllPropertyVariations : TestDomainBase
  {
    public ClassWithAllPropertyVariations (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    // methods and properties

    public ClassWithAllPropertyVariations Parent
    {
      get { return null; }
      set { }
    }

    public ObjectList<ClassWithAllPropertyVariations> Children
    {
      get { return null; }
      set { }
    }
  }
}
