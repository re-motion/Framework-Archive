namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  public abstract class OrderWithNewPropertyAccessBase: DomainObject
  {
    protected OrderWithNewPropertyAccessBase()
    {
    }

    protected OrderWithNewPropertyAccessBase (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public string BaseProperty
    {
      get { return string.Empty; }
      set { }
    }
  }
}