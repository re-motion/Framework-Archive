using System;
using System.Collections;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
//TODO: make usage of nvarchar/varchar for string properties configurable (app.config)
public class SqlBuilder : IBuilder
{
  // types

  // static members and constants

  // member fields

  private string _outputFile;
  private string _mappingFile = MappingLoader.DefaultConfigurationFile;
  private string _mappingSchemaFile = MappingLoader.DefaultSchemaFile;
  private string _storageProviderFile = StorageProviderConfigurationLoader.DefaultConfigurationFile;
  private string _storageProviderSchemaFile = StorageProviderConfigurationLoader.DefaultSchemaFile;

  private IBuilder[] _builders;

  // construction and disposing

  public SqlBuilder (string outputFile) : this (outputFile, null, null, null, null)
  {
  }

	public SqlBuilder (string outputFile, string mappingFile, string mappingSchemaFile, string storageProvidersFile, string storageProvidersSchemaFile)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("outputFile", outputFile);
    
    _outputFile = outputFile;

    if (mappingFile != null)
      _mappingFile = mappingFile;
    if (mappingSchemaFile != null)
      _mappingSchemaFile = mappingSchemaFile;
    if (storageProvidersFile != null)
      _storageProviderFile = storageProvidersFile;
    if (storageProvidersSchemaFile != null)
      _storageProviderSchemaFile = storageProvidersSchemaFile;

    MappingConfiguration.SetCurrent (new MappingConfiguration (_mappingFile, _mappingSchemaFile));

    StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (_storageProviderFile, _storageProviderSchemaFile));

    ArrayList builders = new ArrayList ();

    string[] storageProviderIDs = GetDistinctStorageProviderIDs ();

    foreach (string storageProviderID in storageProviderIDs)
    {
      string filename = _outputFile;
      if (storageProviderIDs.Length > 1)
      {
        if (filename.IndexOf (".") > -1)
          filename = filename.Insert (filename.LastIndexOf ("."), "_" + storageProviderID);
        else
          filename = filename + "_" + storageProviderID;
      }

      builders.Add (new SqlFileBuilder (filename, storageProviderID));
    }
    _builders = (IBuilder[]) builders.ToArray (typeof (IBuilder));
	}

  // methods and properties

  #region IBuilder Members

  public void Build()
  {
    foreach (IBuilder builder in _builders)
      builder.Build ();
  }

  #endregion

  #region IDisposable Members

  public void Dispose()
  {
    // TODO:  Add SqlBuilder.Dispose implementation
  }

  #endregion

  private string[] GetDistinctStorageProviderIDs ()
  {
    ArrayList storageProviderIDs = new ArrayList();
    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      if (!storageProviderIDs.Contains (classDefinition.StorageProviderID))
        storageProviderIDs.Add (classDefinition.StorageProviderID);
    }
    return (string[]) storageProviderIDs.ToArray (typeof (string));
  }
}
}
