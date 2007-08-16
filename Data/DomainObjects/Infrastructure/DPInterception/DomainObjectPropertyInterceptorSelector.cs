using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Data.DomainObjects.Infrastructure.Interception;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception
{
  [Serializable]
  internal class DomainObjectPropertyInterceptorSelector : IInterceptorSelector<DomainObject>
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

      if (!MappingConfiguration.Current.ClassDefinitions.Contains (type))
        return false;

      if (ReflectionUtility.IsPropertyAccessor (memberInfo))
      {
        PropertyInfo property = ReflectionUtility.GetPropertyForMethod (memberInfo);
        string id = DomainObjectPropertyInterceptor.GetIdentifierFromProperty (property);

        ClassDefinition classDefinition = GetClassDefinitionIfRelevantProperty (type, id);

        if (classDefinition == null && memberInfo.IsAbstract)
        {
          throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0}, property {1} is abstract "
              + "but not defined in the mapping (assumed property id: {2}).", type.FullName, property.Name, id), type);
        }

        if (classDefinition != null && memberInfo.IsAbstract && ReflectionUtility.IsPropertySetter (memberInfo)
            && PropertyAccessor.GetPropertyKind(classDefinition, id) == PropertyKind.RelatedObjectCollection)
        {
          throw new NonInterceptableTypeException (string.Format ("Cannot instantiate type {0}, automatic properties for related object collections "
              + "cannot have setters: property '{1}', property id '{2}'.", type.FullName, property.Name, id), type);
        }

        return classDefinition != null;
      }
      else
        return false;
    }

    /// <summary>
    /// Returns the relevant class definition for the given property data, or null if this is not a valid mapping property.
    /// </summary>
    /// <param name="type">The type containing the property.</param>
    /// <param name="propertyID">The property identifier.</param>
    /// <returns>The class devinition for the property, or null if it is not a valid mapping property.</returns>
    public static ClassDefinition GetClassDefinitionIfRelevantProperty (Type type, string propertyID)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("propertyID", propertyID);

      if (!MappingConfiguration.Current.ClassDefinitions.Contains (type))
        return null;
      else
      {
        ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
        return PropertyAccessor.IsValidProperty (classDefinition, propertyID) ? classDefinition : null;
      }
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
