using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{

public class DomainModelBuilder
{
  private static readonly string s_fileExtention = ".cs";

	public static void Build (
      string outputFolder,
      string domainObjectBaseClass, 
      string domainObjectCollectionBaseClass,
      bool serializableAttribute,
      bool multiLingualResourcesAttribute)    
	{
    ArgumentUtility.CheckNotNull ("outputFolder", outputFolder);

    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      DomainObjectBuilder.Build (
          GetFileName (outputFolder, classDefinition.ClassType), 
          classDefinition, domainObjectBaseClass, serializableAttribute, multiLingualResourcesAttribute);

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
      {
        if (endPointDefinition.Cardinality != CardinalityType.Many)
          continue;

        if (endPointDefinition.PropertyType.Name == DomainObjectCollectionBuilder.DefaultBaseClass)
          continue;

        Type requiredItemType = classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName).ClassType;

        DomainObjectCollectionBuilder.Build (
            GetFileName (outputFolder, endPointDefinition.PropertyType), 
            endPointDefinition.PropertyType, 
            requiredItemType.Name,
            domainObjectCollectionBaseClass, serializableAttribute);
      }

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (propertyDefinition.PropertyType.IsEnum && propertyDefinition.PropertyType.DeclaringType == null)
        {
          EnumBuilder.Build (
              GetFileName (outputFolder, propertyDefinition.PropertyType), propertyDefinition.PropertyType, multiLingualResourcesAttribute);
        }
      }
    }
  }

  private static string GetFileName (string outputFolder, Type type)
  {
    return Path.Combine (outputFolder, type.Name + s_fileExtention);
  }

  private DomainModelBuilder()
  {
  }
}

}
