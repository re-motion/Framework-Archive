using System;
using Remotion.Collections;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  // TODO: Why have a different SearchObjectProvider? Remove this class?
  public class SearchObjectProvider : BusinessObjectProvider
  {
    private static readonly SearchObjectProvider s_instance = new SearchObjectProvider();

    public static SearchObjectProvider Instance
    {
      get { return s_instance; }
    }

    private SearchObjectProvider ()
    {
    }

    private readonly InterlockedCache<Type, IBusinessObjectService> _serviceCache = new InterlockedCache<Type, IBusinessObjectService>();

    protected override ICache<Type, IBusinessObjectService> ServiceCache
    {
      get { return _serviceCache; }
    }
  }
}