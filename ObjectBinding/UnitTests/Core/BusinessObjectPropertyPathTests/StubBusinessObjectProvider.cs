using System;
using Remotion.Collections;
using Remotion.ObjectBinding;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectPropertyPathTests
{
  public class StubBusinessObjectProvider : BusinessObjectProvider
  {
    protected override ICache<Type, IBusinessObjectService> ServiceCache
    {
      get { throw new NotImplementedException(); }
    }
  }
}