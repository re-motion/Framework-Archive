using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
  public class SqlBuilder
  {
    // types

    // static members and constants

    public static void Build (MappingConfiguration mappingConfiguration, StorageProviderConfiguration storageProviderConfiguration, string outputFolder)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("outputFolder", outputFolder);

      string[] storageProviderIDs = GetDistinctStorageProviderIDs (mappingConfiguration);

      if (storageProviderIDs.Length == 1)
      {
        string fileName = Path.Combine (outputFolder, "SetupDB.sql");
        SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (mappingConfiguration, storageProviderConfiguration, storageProviderIDs[0]);
        File.WriteAllText (fileName, sqlFileBuilder.GetScript ());
      }
      else
      {
        foreach (string storageProviderID in storageProviderIDs)
        {
          string fileName = "SetupDB_" + storageProviderID + ".sql";

          SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (mappingConfiguration, storageProviderConfiguration, storageProviderID);
          File.WriteAllText (fileName, sqlFileBuilder.GetScript ());
        }
      }
    }

    private static string[] GetDistinctStorageProviderIDs (MappingConfiguration mappingConfiguration)
    {
      List<String> storageProviderIDs = new List<String> ();
      foreach (ClassDefinition classDefinition in mappingConfiguration.ClassDefinitions)
      {
        if (!storageProviderIDs.Contains (classDefinition.StorageProviderID))
          storageProviderIDs.Add (classDefinition.StorageProviderID);
      }
      return storageProviderIDs.ToArray ();
    }

    // member fields

    // construction and disposing

    private SqlBuilder ()
    {
    }

    // methods and properties

  }

}
