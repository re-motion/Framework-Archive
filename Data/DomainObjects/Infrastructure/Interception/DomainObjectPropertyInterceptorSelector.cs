using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  class DomainObjectPropertyInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    private DomainObjectPropertyInterceptor _interceptor;

    public DomainObjectPropertyInterceptorSelector (DomainObjectPropertyInterceptor interceptor)
    {
      _interceptor = interceptor;
    }

    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      if (!MappingConfiguration.Current.ClassDefinitions.Contains(type))
      {
        return false;
      }

      if (ReflectionUtility.IsPropertyAccessor (memberInfo))
      {
        PropertyInfo property = ReflectionUtility.GetPropertyForMethod (memberInfo);
        //if (memberInfo.IsAbstract && !Attribute.IsDefined (property, typeof (AutomaticPropertyAttribute), true))
        //  return false;

        // this interceptor only intercepts properties which are defined in the mapping either as a property definition or as a related object
        string id = DomainObjectPropertyInterceptor.GetIdentifierFromProperty (property);
        bool isDefined = DomainObjectPropertyInterceptor.IsPropertyValue (type, id) || DomainObjectPropertyInterceptor.IsRelatedObject (type, id);
        if (!isDefined && memberInfo.IsAbstract)
        {
          throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0}, property {1} is abstract "
              + "but not defined in the mapping (assumed property id: {2}).", property.DeclaringType.FullName, property.Name, id), type);
        }
        return isDefined;
      }
      else
        return false;
    }

    public IInterceptor<DomainObject> SelectInterceptor (Type type, MethodInfo memberInfo)
    {
      Assertion.DebugAssert (ShouldInterceptMethod (type, memberInfo));
      return _interceptor;
    }
  }
}
