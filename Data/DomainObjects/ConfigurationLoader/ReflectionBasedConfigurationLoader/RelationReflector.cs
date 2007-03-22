using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class RelationReflector
  {
    public RelationReflector()
    {
    }

    public IRelationEndPointDefinition GetRelationEndPointDefinition (PropertyInfo propertyInfo, PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return new RelationEndPointDefinition (propertyDefinition.ClassDefinition, propertyDefinition.PropertyName, !propertyDefinition.IsNullable);
    }
  }
}