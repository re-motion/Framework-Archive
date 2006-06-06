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
        MappingConfiguration mappingConfiguration,
        string outputFolder,
        string domainObjectBaseClass,
        string domainObjectCollectionBaseClass,
        bool serializableAttribute,
        bool multiLingualResourcesAttribute)
    {
      ArgumentUtility.CheckNotNull ("outputFolder", outputFolder);

      foreach (ClassDefinition classDefinition in mappingConfiguration.ClassDefinitions)
      {
        DomainObjectBuilder.Build (
            mappingConfiguration, GetFileName (outputFolder, new TypeName (classDefinition.ClassTypeName)),
            classDefinition, domainObjectBaseClass, serializableAttribute, multiLingualResourcesAttribute);

        foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
        {
          if (endPointDefinition.Cardinality != CardinalityType.Many)
            continue;

          TypeName propertyTypeName = new TypeName (endPointDefinition.PropertyTypeName);

          if (DomainObjectCollectionBuilder.DefaultBaseTypeName.CompareTo (propertyTypeName) == 0)
            continue;

          TypeName requiredItemTypeName = new TypeName (classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName).ClassTypeName);

          DomainObjectCollectionBuilder.Build (
              GetFileName (outputFolder, propertyTypeName),
              propertyTypeName,
              requiredItemTypeName.Name,
              domainObjectCollectionBaseClass, serializableAttribute);
        }

        foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        {
          if (propertyDefinition.MappingTypeName.Contains (","))
          {
            TypeName typeName = new TypeName (propertyDefinition.MappingTypeName);

            if (typeName.DeclaringTypeName == null)
              EnumBuilder.Build (GetFileName (outputFolder, typeName), typeName, multiLingualResourcesAttribute);
          }
        }
      }
    }

    private static string GetFileName (string outputFolder, TypeName typeName)
    {
      return Path.Combine (outputFolder, typeName.Name + s_fileExtention);
    }

    private DomainModelBuilder ()
    {
    }
  }
}
