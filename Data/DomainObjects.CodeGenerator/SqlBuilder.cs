using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class SqlBuilder : ConfigurationBasedBuilder
{
  // types

  // static members and constants

  // member fields

  private string _outputFile;

  private IBuilder[] _builders;

  // construction and disposing

	public SqlBuilder (string outputFile, string xmlFilePath, string xmlSchemaFilePath) : base (xmlFilePath, xmlSchemaFilePath)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("outputFile", outputFile);
    
    _outputFile = outputFile;

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

  protected override void Dispose (bool disposing)
  {
    if (!Disposed)
    {
      if (disposing)
      {
        foreach (IBuilder builder in _builders)
          builder.Dispose ();
        _builders = null;
      }
      Disposed = true;
    }
  }

  // methods and properties

  public override void Build ()
  {
    foreach (IBuilder builder in _builders)
      builder.Build ();
  }

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
