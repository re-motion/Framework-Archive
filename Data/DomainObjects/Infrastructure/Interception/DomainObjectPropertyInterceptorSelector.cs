using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.Interception
{
  [Serializable]
  public class DomainObjectPropertyInterceptorSelector : IInterceptorSelector<DomainObject>
  {
    private DomainObjectPropertyInterceptor _interceptor;

    public DomainObjectPropertyInterceptorSelector (DomainObjectPropertyInterceptor interceptor)
    {
      ArgumentUtility.CheckNotNull ("interceptor", interceptor);
      _interceptor = interceptor;
    }

    //TODO: Write tests that verfiy that type is used instead of property.PropertyType
    public bool ShouldInterceptMethod (Type type, MethodInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      if (!MappingConfiguration.Current.ClassDefinitions.Contains(type))
      {
        return false;
      }

      if (ReflectionUtility.IsPropertyAccessor (memberInfo))
      {
        PropertyInfo property = ReflectionUtility.GetPropertyForMethod (memberInfo);
        string id = DomainObjectPropertyInterceptor.GetIdentifierFromProperty (property);

        bool isDefined = DomainObjectPropertyInterceptor.IsInterceptable (type, id);
        if (!isDefined && memberInfo.IsAbstract)
        {
          throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0}, property {1} is abstract "
              + "but not defined in the mapping (assumed property id: {2}).", type.FullName, property.Name, id), type);
        }

        if (ReflectionUtility.IsPropertySetter(memberInfo) && DomainObjectPropertyInterceptor.IsRelatedObjectCollection (type, id))
        {
          throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0}, automatic properties for related object collections "
              + "cannot have setters: property '{1}', property id '{2}'.", type.FullName, property.Name, id), type);
        }

        return isDefined;
      }
      else
        return false;
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
