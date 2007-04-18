using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  [Serializable]
  public class DomainObjectInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    public readonly DomainObjectTypeInterceptor TypeInterceptor = new DomainObjectTypeInterceptor ();
    public readonly DomainObjectPropertyInterceptor PropertyInterceptor = new DomainObjectPropertyInterceptor ();

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      bool hasInterceptor = (TypeInterceptor.Selector.ShouldInterceptMethod (type, memberInfo) || PropertyInterceptor.Selector.ShouldInterceptMethod (type, memberInfo));
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
      if (TypeInterceptor.Selector.ShouldInterceptMethod (type, memberInfo))
      {
        return TypeInterceptor.Selector.SelectInterceptor (type, memberInfo);
      }
      else
      {
        return PropertyInterceptor.Selector.SelectInterceptor (type, memberInfo);
      }
    }
  }
}
