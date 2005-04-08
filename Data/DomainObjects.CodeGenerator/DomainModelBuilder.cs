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

    ArrayList domainObjectBuilders = new ArrayList ();
    ArrayList domainObjectCollectionBuilders = new ArrayList ();

    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      domainObjectBuilders.Add (new DomainObjectBuilder (
          Path.Combine (outputFolder, classDefinition.ClassType.Name + s_fileExtention), 
          classDefinition, domainObjectBaseClass));

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
      {
        if (endPointDefinition.Cardinality != CardinalityType.Many)
          continue;

        if (endPointDefinition.PropertyType.Name == DomainObjectCollectionBuilder.DefaultBaseClass)
          continue;

        Type requiredItemType = classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName).ClassType;

        domainObjectCollectionBuilders.Add (new DomainObjectCollectionBuilder (
            Path.Combine (outputFolder, endPointDefinition.PropertyType.Name + s_fileExtention), 
            endPointDefinition.PropertyType, 
            requiredItemType.Name,
            domainObjectCollectionBaseClass));
      }
    }
    _domainObjectBuilders = (IBuilder[]) domainObjectBuilders.ToArray (typeof (IBuilder));
    _domainObjectCollectionBuilders = (IBuilder[]) domainObjectCollectionBuilders.ToArray (typeof (IBuilder));
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
    foreach (IBuilder builder in _domainObjectBuilders)
      builder.Build ();
    foreach (IBuilder builder in _domainObjectCollectionBuilders)
      builder.Build ();
  }
}
}
