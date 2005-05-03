using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class DomainModelBuilder : ConfigurationBasedBuilder
{
  // types

  // static members and constants

  private static readonly string s_fileExtention = ".cs";

  // member fields

  private IBuilder[] _enumBuilders;
  private IBuilder[] _domainObjectBuilders;
  private IBuilder[] _domainObjectCollectionBuilders;

  // construction and disposing

	public DomainModelBuilder (
      string outputFolder,
      string xmlFilePath,
      string xmlSchemaFilePath,
      string assemblyPath,
      string domainObjectBaseClass, 
      string domainObjectCollectionBaseClass)
    : base (xmlFilePath, xmlSchemaFilePath, assemblyPath)
	{
    ArgumentUtility.CheckNotNull ("outputFolder", outputFolder);

    ArrayList enumBuilders = new ArrayList ();
    ArrayList domainObjectBuilders = new ArrayList ();
    ArrayList domainObjectCollectionBuilders = new ArrayList ();

    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      domainObjectBuilders.Add (new DomainObjectBuilder (
          GetFileName (outputFolder, classDefinition.ClassType), 
          classDefinition, domainObjectBaseClass));

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
      {
        if (endPointDefinition.Cardinality != CardinalityType.Many)
          continue;

        if (endPointDefinition.PropertyType.Name == DomainObjectCollectionBuilder.DefaultBaseClass)
          continue;

        Type requiredItemType = classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName).ClassType;

        domainObjectCollectionBuilders.Add (new DomainObjectCollectionBuilder (
            GetFileName (outputFolder, endPointDefinition.PropertyType), 
            endPointDefinition.PropertyType, 
            requiredItemType.Name,
            domainObjectCollectionBaseClass));
      }

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      {
        if (propertyDefinition.PropertyType.IsEnum && propertyDefinition.PropertyType.DeclaringType == null)
        {
          enumBuilders.Add (new EnumBuilder (
              GetFileName (outputFolder, propertyDefinition.PropertyType), propertyDefinition.PropertyType));
        }
      }
    }
    _enumBuilders = (IBuilder[]) enumBuilders.ToArray (typeof (IBuilder));
    _domainObjectBuilders = (IBuilder[]) domainObjectBuilders.ToArray (typeof (IBuilder));
    _domainObjectCollectionBuilders = (IBuilder[]) domainObjectCollectionBuilders.ToArray (typeof (IBuilder));
  }

  private string GetFileName (string outputFolder, Type type)
  {
    return Path.Combine (outputFolder, type.Name + s_fileExtention);
  }

  protected override void Dispose (bool disposing)
  {
    if(!Disposed)
    {
      if(disposing)
      {
        foreach (IBuilder builder in _domainObjectBuilders)
          builder.Dispose ();
        _domainObjectBuilders = null;

        foreach (IBuilder builder in _domainObjectCollectionBuilders)
          builder.Dispose ();
        _domainObjectCollectionBuilders = null;
      }
    }
    Disposed = true;
  }

  // methods and properties

  public override void Build ()
  {
    foreach (IBuilder builder in _enumBuilders)
      builder.Build ();
    foreach (IBuilder builder in _domainObjectBuilders)
      builder.Build ();
    foreach (IBuilder builder in _domainObjectCollectionBuilders)
      builder.Build ();
  }
}
}
