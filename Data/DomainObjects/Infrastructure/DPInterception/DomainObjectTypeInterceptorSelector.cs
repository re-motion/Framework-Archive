using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception
{
  [Serializable]
  internal class DomainObjectTypeInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    private DomainObjectTypeInterceptor _interceptor;

    public DomainObjectTypeInterceptorSelector (DomainObjectTypeInterceptor interceptor)
    {
      ArgumentUtility.CheckNotNull ("interceptor", interceptor);
      _interceptor = interceptor;
    }

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      return memberInfo.Name == "GetPublicDomainObjectType";
    }

    public IInterceptor<DomainObject> SelectInterceptor (Type type, MethodInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      Assertion.DebugAssert (ShouldInterceptMethod (type, memberInfo));
      return _interceptor;
    }
  }
}
