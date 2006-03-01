using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{

public class SqlBuilder 
{
	public static void Build (string outputFile) 
	{
    ArgumentUtility.CheckNotNullOrEmpty ("outputFile", outputFile);

    string[] storageProviderIDs = GetDistinctStorageProviderIDs ();

    if (storageProviderIDs.Length == 1)
    {
      SqlFileBuilder.Build (outputFile, storageProviderIDs[0]);
    }
    else
    {
      foreach (string storageProviderID in storageProviderIDs)
      {
        string filename = outputFile;
        if (filename.IndexOf (".") > -1)
          filename = filename.Insert (filename.LastIndexOf ("."), "_" + storageProviderID);
        else
          filename = filename + "_" + storageProviderID;

        SqlFileBuilder.Build (filename, storageProviderID);
      }
    }
	}

  private static string[] GetDistinctStorageProviderIDs ()
  {
    ArrayList storageProviderIDs = new ArrayList();
    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
    {
      if (!storageProviderIDs.Contains (classDefinition.StorageProviderID))
        storageProviderIDs.Add (classDefinition.StorageProviderID);
    }
    return (string[]) storageProviderIDs.ToArray (typeof (string));
  }

  private SqlBuilder()
  {
  }
}

}
