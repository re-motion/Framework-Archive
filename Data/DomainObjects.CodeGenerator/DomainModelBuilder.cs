using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class DomainModelBuilder : IBuilder
{
  // types

  // static members and constants

  private static readonly string s_fileExtention = ".cs";

  // member fields

  private string _outputFolder = "";
  private string _mappingFile;
  private string _mappingSchemaFile;
  private string _storageProvidersFile;
  private string _storageProvidersSchemaFile;
  private IBuilder[] _domainObjectBuilders;
  private IBuilder[] _domainObjectCollectionBuilders;

  // construction and disposing

	public DomainModelBuilder (
      string outputFolder,
      string mappingFile, 
      string mappingSchemaFile, 
      string storageProvidersFile, 
      string storageProvidersSchemaFile)
      : this (outputFolder, mappingFile, mappingSchemaFile, storageProvidersFile, storageProvidersSchemaFile, null, null)
	{
  }

	public DomainModelBuilder (
      string outputFolder,
      string mappingFile, 
      string mappingSchemaFile, 
      string storageProvidersFile, 
      string storageProvidersSchemaFile, 
      string domainObjectBaseClass, 
      string domainObjectCollectionBaseClass)
	{
    ArgumentUtility.CheckNotNull ("outputFolder", outputFolder);
    ArgumentUtility.CheckNotNullOrEmpty ("mappingFile", mappingFile);
    ArgumentUtility.CheckNotNullOrEmpty ("mappingSchemaFile", mappingSchemaFile);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProvidersFile", storageProvidersFile);
    ArgumentUtility.CheckNotNullOrEmpty ("storageProvidersSchemaFile", storageProvidersSchemaFile);

    if (outputFolder != string.Empty)
      _outputFolder = outputFolder;

    _mappingFile = mappingFile;
    _mappingSchemaFile = mappingSchemaFile;
    _storageProvidersFile = storageProvidersFile;
    _storageProvidersSchemaFile = storageProvidersSchemaFile;

    ArrayList domainObjectBuilders = new ArrayList ();
    ArrayList domainObjectCollectionBuilders = new ArrayList ();

    MappingConfiguration mappingConfiguration = MappingConfiguration.Current;
    foreach (ClassDefinition classDefinition in mappingConfiguration.ClassDefinitions)
    {
      domainObjectBuilders.Add (new DomainObjectBuilder (
          Path.Combine (_outputFolder, classDefinition.ClassType.Name + s_fileExtention), 
          classDefinition, domainObjectBaseClass));

      foreach (IRelationEndPointDefinition endPointDefinition in classDefinition.GetMyRelationEndPointDefinitions ())
      {
        if (endPointDefinition.Cardinality != CardinalityType.Many)
          continue;

        if (endPointDefinition.PropertyType.Name == DomainObjectCollectionBuilder.DefaultBaseClass)
          continue;

        Type requiredItemType = classDefinition.GetOppositeClassDefinition (endPointDefinition.PropertyName).ClassType;

        domainObjectCollectionBuilders.Add (new DomainObjectCollectionBuilder (
            Path.Combine (_outputFolder, endPointDefinition.PropertyType.Name + s_fileExtention), 
            endPointDefinition.PropertyType, 
            requiredItemType.Name,
            domainObjectCollectionBaseClass));
      }
    }
    _domainObjectBuilders = (IBuilder[]) domainObjectBuilders.ToArray (typeof (IBuilder));
    _domainObjectCollectionBuilders = (IBuilder[]) domainObjectCollectionBuilders.ToArray (typeof (IBuilder));
  }

  // methods and properties

  #region IBuilder Members

  public void Build()
  {
    foreach (IBuilder builder in _domainObjectBuilders)
      builder.Build ();
    foreach (IBuilder builder in _domainObjectCollectionBuilders)
      builder.Build ();
  }

  #endregion
}
}
