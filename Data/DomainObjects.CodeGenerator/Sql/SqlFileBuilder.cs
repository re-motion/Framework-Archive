using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using System.IO;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public class SqlFileBuilder
  {
    // types

    // static members and constants

    public const string DefaultSchema = "dbo";

    public static void Build (MappingConfiguration mappingConfiguration, StorageProviderConfiguration storageProviderConfiguration, string outputPath)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("storageProviderConfiguration", storageProviderConfiguration);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      if (outputPath != string.Empty && !Directory.Exists (outputPath))
        Directory.CreateDirectory (outputPath);

      if (storageProviderConfiguration.StorageProviderDefinitions.Count == 1)
      {
        RdbmsProviderDefinition rdbmsProviderDefinition = storageProviderConfiguration.StorageProviderDefinitions[0] as RdbmsProviderDefinition;
        if (rdbmsProviderDefinition != null)
          Build (mappingConfiguration, rdbmsProviderDefinition, GetFileName (rdbmsProviderDefinition, outputPath, false));
      }
      else
      {
        foreach (StorageProviderDefinition storageProviderDefinition in storageProviderConfiguration.StorageProviderDefinitions)
        {
          RdbmsProviderDefinition rdbmsProviderDefinition = storageProviderDefinition as RdbmsProviderDefinition;
          if (rdbmsProviderDefinition != null)
            Build (mappingConfiguration, rdbmsProviderDefinition, GetFileName (rdbmsProviderDefinition, outputPath, true));
        }
      }
    }

    public static void Build (
        MappingConfiguration mappingConfiguration, 
        RdbmsProviderDefinition rdbmsProviderDefinition, 
        string fileName)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      SqlFileBuilder sqlFileBuilder = new SqlFileBuilder (mappingConfiguration, rdbmsProviderDefinition);
      File.WriteAllText (fileName, sqlFileBuilder.GetScript ());
    }

    public static string GetFileName (StorageProviderDefinition storageProviderDefinition, string outputPath, bool multipleStorageProviders)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("outputPath", outputPath);

      string fileName;
      if (multipleStorageProviders)
        fileName = string.Format ("SetupDB_{0}.sql", storageProviderDefinition.ID);
      else
        fileName = "SetupDB.sql";

      return Path.Combine (outputPath,  fileName);
    }

    // member fields

    private MappingConfiguration _mappingConfiguration;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;

    private ClassDefinitionCollection _classes;

    // construction and disposing

    public SqlFileBuilder (
        MappingConfiguration mappingConfiguration,
        RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("mappingConfiguration", mappingConfiguration);
      ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition);

      _mappingConfiguration = mappingConfiguration;
      _rdbmsProviderDefinition = rdbmsProviderDefinition;
      _classes = GetClassesInStorageProvider (mappingConfiguration.ClassDefinitions, _rdbmsProviderDefinition.ID);
    }

    private ClassDefinitionCollection GetClassesInStorageProvider (ClassDefinitionCollection classDefinitions, string storageProviderID)
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      foreach (ClassDefinition currentClass in classDefinitions)
      {
        if (currentClass.StorageProviderID == _rdbmsProviderDefinition.ID)
          classes.Add (currentClass);
      }

      return classes;
    }

    // methods and properties

    public MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    public RdbmsProviderDefinition RdbmsProviderDefinition
    {
      get { return _rdbmsProviderDefinition; }
    }

    public ClassDefinitionCollection Classes
    {
      get { return _classes; }
    }

    public string GetDatabaseName ()
    {
      //TODO improve this logic
      string temp = _rdbmsProviderDefinition.ConnectionString.Substring (_rdbmsProviderDefinition.ConnectionString.IndexOf ("Initial Catalog=") + 16);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    public string GetScript ()
    {
      ViewBuilder viewBuilder = new ViewBuilder ();
      viewBuilder.AddViews (_classes);

      TableBuilder tableBuilder = new TableBuilder ();
      tableBuilder.AddTables (_classes);

      ConstraintBuilder constraintBuilder = new ConstraintBuilder ();
      constraintBuilder.AddConstraints (_classes);

      return string.Format ("USE {0}\n"
          + "GO\n\n"
          + "-- Drop all views that will be created below\n"
          + "{1}GO\n\n"
          + "-- Drop foreign keys of all tables that will be created below\n"
          + "{2}GO\n\n"
          + "-- Drop all tables that will be created below\n"
          + "{3}GO\n\n"
          + "-- Create all tables\n"
          + "{4}GO\n\n"
          + "-- Create constraints for tables that were created above\n"
          + "{5}GO\n\n"
          + "-- Create a view for every class\n"
          + "{6}GO\n", 
          GetDatabaseName (), 
          viewBuilder.GetDropViewScript (),
          constraintBuilder.GetDropConstraintScript (), 
          tableBuilder.GetDropTableScript (),
          tableBuilder.GetCreateTableScript (),
          constraintBuilder.GetAddConstraintScript (), 
          viewBuilder.GetCreateViewScript ());
    }
  }
}
