using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="RelationPropertyFinder"/> is used to find all <see cref="PropertyInfo"/> objects that constitute a 
  /// <see cref="RelationEndPointDefinition"/>.
  /// </summary>
  public class RelationPropertyFinder : PropertyFinderBase
  {
    public RelationPropertyFinder (Type type, bool includeBaseProperties)
        : base (type, includeBaseProperties)
    {
    }

    protected override bool FindPropertiesFilter (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      if (!base.FindPropertiesFilter (propertyInfo))
        return false;

      return IsRelationEndPoint (propertyInfo);
    }

    private bool IsRelationEndPoint (PropertyInfo propertyInfo)
    {
      return typeof (DomainObject).IsAssignableFrom (propertyInfo.PropertyType) || ReflectionUtility.IsObjectList (propertyInfo.PropertyType);
    }
  }
}