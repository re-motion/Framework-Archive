using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Interception
{
  class DomainObjectInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    private DomainObjectTypeInterceptor _typeInterceptor = new DomainObjectTypeInterceptor ();
    private DomainObjectPropertyInterceptor _propertyInterceptor = new DomainObjectPropertyInterceptor ();

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      if (_typeInterceptor.Selector.ShouldInterceptMethod (type, memberInfo) || _propertyInterceptor.Selector.ShouldInterceptMethod (type, memberInfo))
      {
        return true;
      }
      else if (memberInfo.IsAbstract)
      {
        throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0}, the method {1} is abstract (and not part of an "
            + "automatic property).", type.FullName, memberInfo.Name), type);
      }
      else
      {
        return false;
      }
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
