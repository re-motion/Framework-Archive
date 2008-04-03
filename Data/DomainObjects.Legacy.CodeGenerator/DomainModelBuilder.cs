using System;
using System.IO;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator
{
  public class DomainModelBuilder
  {
    private static readonly string s_fileExtention = ".cs";

    public DomainModelBuilder ()
    {
    }

    public void Build (
        MappingConfiguration mappingConfiguration,
        string outputFolder,
        string domainObjectBaseClass,
        string domainObjectCollectionBaseClass,
        bool serializableAttribute,
        bool multiLingualResourcesAttribute)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("outputFolder", outputFolder);
    
      foreach (XmlBasedClassDefinition classDefinition in mappingConfiguration.ClassDefinitions)
      {
        using (TextWriter writer = new StreamWriter (GetFileName (outputFolder, new TypeName (classDefinition.ClassTypeName))))
        {
          DomainObjectBuilder builder = CreateDomainObjectBuilder(mappingConfiguration, writer);
          builder.Build (classDefinition, domainObjectBaseClass, serializableAttribute, multiLingualResourcesAttribute);
        }

        foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
        {
          if (endPointDefinition.Cardinality != CardinalityType.Many)
            continue;

          TypeName propertyTypeName = new TypeName (endPointDefinition.PropertyTypeName);

          if (DomainObjectCollectionBuilder.DefaultBaseTypeName.CompareTo (propertyTypeName) == 0)
            continue;

          TypeName requiredItemTypeName = new TypeName (
              ((XmlBasedClassDefinition) classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName)).ClassTypeName);

          using (TextWriter writer = new StreamWriter (GetFileName (outputFolder, propertyTypeName)))
          {
            DomainObjectCollectionBuilder builder = new DomainObjectCollectionBuilder (writer);
            builder.Build (propertyTypeName, requiredItemTypeName.Name, domainObjectCollectionBaseClass, serializableAttribute);
          }
        }

        foreach (XmlBasedPropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
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

    protected virtual DomainObjectBuilder CreateDomainObjectBuilder (MappingConfiguration mappingConfiguration, TextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("writer", writer);
      
      return new DomainObjectBuilder (mappingConfiguration, writer);
    }

    private string GetFileName (string outputFolder, TypeName typeName)
    {
      return Path.Combine (outputFolder, typeName.Name + s_fileExtention);
    }
  }
}