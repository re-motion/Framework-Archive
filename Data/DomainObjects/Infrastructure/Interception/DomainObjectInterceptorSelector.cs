using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  class DomainObjectInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    private DomainObjectTypeInterceptor _typeInterceptor = new DomainObjectTypeInterceptor ();
    private DomainObjectPropertyInterceptor _propertyInterceptor = new DomainObjectPropertyInterceptor ();

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      bool hasInterceptor = (_typeInterceptor.Selector.ShouldInterceptMethod (type, memberInfo) || _propertyInterceptor.Selector.ShouldInterceptMethod (type, memberInfo));
      if (!hasInterceptor && memberInfo.IsAbstract)
      {
        string message = string.Format ("Cannot instantiate type {0} as its member {1} is abstract (and not an automatic property).",
          type.FullName, memberInfo.Name);
        throw new NonInterceptableTypeException (message, type);
      }
      return hasInterceptor;
    }

    public IInterceptor<DomainObject> SelectInterceptor (Type type, MethodInfo memberInfo)
    {
      if (_typeInterceptor.Selector.ShouldInterceptMethod (type, memberInfo))
      {
        return _typeInterceptor.Selector.SelectInterceptor (type, memberInfo);
      }
      else
      {
        return _propertyInterceptor.Selector.SelectInterceptor (type, memberInfo);
      }
    }
  }
}
