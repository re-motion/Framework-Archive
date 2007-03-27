using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class ClassReflector
  {
    private Type _type;

    public ClassReflector (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));

      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public ClassDefinition GetMetadata()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          Type.FullName,
          Type.Name,
          DomainObjectsConfiguration.Current.Storage.StorageProviderDefinition.Name,
          Type,
          null);

      List<PropertyReflector> propertyReflectors = new List<PropertyReflector>();
      MemberInfo[] propertyInfos = Type.FindMembers (
         MemberTypes.Property,
         BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
         FindPropertiesFilter,
         null);

      foreach (PropertyInfo propertyInfo in propertyInfos)
        propertyReflectors.Add (new PropertyReflector (propertyInfo));

      foreach (PropertyReflector propertyReflector in propertyReflectors)
        classDefinition.MyPropertyDefinitions.Add (propertyReflector.GetMetadata());

      return classDefinition;
    }

    private bool FindPropertiesFilter (MemberInfo member, object filterCriteria)
    {
      PropertyInfo propertyInfo = ArgumentUtility.CheckNotNullAndType<PropertyInfo> ("member", member);

      StorageClassAttribute storageClassAttribute = AttributeUtility.GetCustomAttribute<StorageClassAttribute> (propertyInfo, true);
      if (storageClassAttribute != null && storageClassAttribute.StorageClass == StorageClass.None)
        return false;

      if (typeof (DomainObjectCollection).IsAssignableFrom (propertyInfo.PropertyType))
        return false;

      return true;
    }
  }
}