using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.Interception
{
  class DomainObjectInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    private DomainObjectTypeInterceptor _typeInterceptor = new DomainObjectTypeInterceptor ();
    private PropertyInterceptor _propertyInterceptor = new PropertyInterceptor ();

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      if (_typeInterceptor.Selector.ShouldInterceptMethod (type, memberInfo) || _propertyInterceptor.Selector.ShouldInterceptMethod (type, memberInfo))
      {
        return true;
      }
      else if (memberInfo.IsAbstract)
      {
        throw new InvalidOperationException ("Cannot instantiate type " + type.FullName + ", the method " + memberInfo.Name
              + " is abstract.");
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
