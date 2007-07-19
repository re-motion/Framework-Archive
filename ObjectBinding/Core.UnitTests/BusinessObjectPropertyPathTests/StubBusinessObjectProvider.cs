using System;
using Rubicon.Collections;
using Rubicon.ObjectBinding;

namespace Rubicon.ObjectBinding.UnitTests.BusinessObjectPropertyPathTests
{
  public class StubBusinessObjectProvider : BusinessObjectProvider
  {
    protected override ICache<Type, IBusinessObjectService> ServiceCache
    {
      get { throw new NotImplementedException(); }
    }
  }
}