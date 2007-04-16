using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public class StandardConfiguration: BaseConfiguration
  {
    private static StandardConfiguration s_instance;

    public static StandardConfiguration Instance
    {
      get
      {
        if (s_instance == null)
          throw new InvalidOperationException ("StandardConfiguration has not been Initialized by invoking Initialize()");
        return s_instance;
      }
    }

    public static void Initialize()
    {
      s_instance = new StandardConfiguration();
    }

    private readonly DomainObjectIDs _domainObjectIDs;

    private StandardConfiguration()
    {
      _domainObjectIDs = new DomainObjectIDs();
    }

    public DomainObjectIDs GetDomainObjectIDs()
    {
      return _domainObjectIDs;
    }
  }
}