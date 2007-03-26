using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class RelationReflector : BaseReflector
  {    
    public RelationReflector ()
    {
    }

    public IRelationEndPointDefinition GetRelationEndPointDefinition (PropertyInfo propertyInfo, ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Validate (propertyInfo);
      
      return new RelationEndPointDefinition (classDefinition, GetPropertyName (propertyInfo), !GetNullabilityForReferenceType (propertyInfo));
    }
  }
}