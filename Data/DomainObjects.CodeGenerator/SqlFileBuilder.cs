using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class SqlFileBuilder : IBuilder
{
  // types

  // static members and constants

  // member fields

  private MappingConfiguration _mappingConfig;
  private StorageProviderConfiguration _storageProviderConfig;
  private string _outputFile;

  // construction and disposing

  public SqlFileBuilder (string outputFile) : this (outputFile, null, null, null, null)
	{
  }

  public SqlFileBuilder (string outputFile, string mappingFile, string mappingSchemaFile, string storageProviderFile, string storageProviderSchemaFile)
	{
    if (mappingFile != null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mappingFile", mappingFile);
      ArgumentUtility.CheckNotNullOrEmpty ("mappingSchemaFile", mappingSchemaFile);

      MappingConfiguration.SetCurrent (new MappingConfiguration (mappingFile, mappingSchemaFile));
    }
    _mappingConfig = MappingConfiguration.Current;

    if (storageProviderFile != null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderFile", storageProviderFile);
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderSchemaFile", storageProviderSchemaFile);

      StorageProviderConfiguration.SetCurrent (new StorageProviderConfiguration (storageProviderFile, storageProviderSchemaFile));
    }
    _storageProviderConfig = StorageProviderConfiguration.Current;

    _outputFile = outputFile;
	}

  // methods and properties

  public void Build ()
  {
    string[] storageProviderIDs = GetDistinctStorageProviderIDs ();

    foreach (string storageProviderID in storageProviderIDs)
    {
      string filename = _outputFile;
      if (storageProviderIDs.Length > 1)
        filename = filename.Insert (filename.LastIndexOf ("."), "_" + storageProviderID);

      CreateSqlFile (filename, storageProviderID);
    }
  }

  private void CreateSqlFile (string filename, string storageProviderID)
  {
    using (StreamWriter writer = new StreamWriter (filename))
    {
      SqlBuilder builder = new SqlBuilder (writer, GetDatabasename (storageProviderID));

      foreach (ClassDefinition baseClass in GetBaseClassDefinitions (storageProviderID))
      {
        builder.DropTable (baseClass.EntityName);
      }

      foreach (ClassDefinition baseClass in GetBaseClassDefinitions (storageProviderID))
      {
        ArrayList derivedClasses = GetDerivedClassDefinitions (baseClass, true);
        bool hasChildClasses = derivedClasses.Count != 0;

        builder.CreateTable (baseClass.EntityName, hasChildClasses, GetColumns (baseClass, derivedClasses));
      }

      foreach (ClassDefinition baseClass in GetBaseClassDefinitions (storageProviderID))
      {
        ConstraintDefinition[] constraints = GetConstraintsRecursive (baseClass);
        builder.AddConstraints (constraints);
      }

      writer.Close ();
    }
  }

  private ConstraintDefinition[] GetConstraintsRecursive (ClassDefinition classDefinition)
  {
    ArrayList constraints = new ArrayList();
    foreach (PropertyDefinition property in classDefinition.MyPropertyDefinitions)
    {
      if (property.MappingType != "objectID") 
        continue;
      ClassDefinition oppositeClass = GetOppositeClass (classDefinition, property.PropertyName);

      if (classDefinition.StorageProviderID == oppositeClass.StorageProviderID)
        constraints.Add (new ConstraintDefinition (classDefinition.ID, oppositeClass.ID, classDefinition.EntityName, property.ColumnName, oppositeClass.EntityName, "ID"));
    }

    foreach (ClassDefinition derivedClass in GetDerivedClassDefinitions (classDefinition))
      constraints.AddRange (GetConstraintsRecursive (derivedClass));

    return (ConstraintDefinition[]) constraints.ToArray (typeof (ConstraintDefinition));
  }

  // TODO: improve this code with existing functionality from RPF
  private ColumnDefinition[] GetColumns (ClassDefinition baseClass, ArrayList derivedClasses)
  {
    ArrayList columns = new ArrayList ();
    MergeColumnDefinitions (columns, baseClass);
    foreach (ClassDefinition derivedClass in derivedClasses)
      MergeColumnDefinitions (columns, derivedClass);

    return (ColumnDefinition[]) columns.ToArray (typeof (ColumnDefinition));
  }

  private void MergeColumnDefinitions (ArrayList columns, ClassDefinition classDefinition)
  {
    foreach (PropertyDefinition property in classDefinition.MyPropertyDefinitions)
    {
      string dataType = BuilderUtility.GetDBType (property.MappingType, property.PropertyType, property.MaxLength);
      if (property.MappingType == "objectID" && 
          GetOppositeClass (classDefinition, property.PropertyName).StorageProviderID != classDefinition.StorageProviderID)
      {
        dataType = SqlBuilder.FullObjectIdDatabaseType;
      }

      columns.Add (new ColumnDefinition (
          property.ColumnName, dataType, property.IsNullable, classDefinition.ID));

      if (property.PropertyType == typeof (ObjectID) 
          && IsOppositeClassInHierarchy (classDefinition, property.PropertyName)
          && GetOppositeClass (classDefinition, property.PropertyName).StorageProviderID == classDefinition.StorageProviderID)
      {
        columns.Add (new ColumnDefinition (property.ColumnName + "ClassID", SqlBuilder.ClassIdDatabaseType, false, classDefinition.ID));
      }
    }
  }

  private ClassDefinition GetOppositeClass (ClassDefinition classDefinition, string propertyName)
  {
    return classDefinition.GetOppositeEndPointDefinition (propertyName).ClassDefinition;;
  }

  private bool IsOppositeClassInHierarchy (ClassDefinition classDefinition, string propertyName)
  {
    ClassDefinition oppositeClass = GetOppositeClass (classDefinition, propertyName);

    //TODO: use ClassDefinition.IsPartOfInheritanceHierarchy instead of the code below
    if (oppositeClass.BaseClass != null)
      return true;

    foreach (ClassDefinition myClassDefinition in _mappingConfig.ClassDefinitions)
    {
      if (myClassDefinition.BaseClass == oppositeClass)
        return true;
    }
    return false;
  }

  private ArrayList GetBaseClassDefinitions (string storageProviderID)
  {
    ArrayList baseClasses = new ArrayList ();
    foreach (ClassDefinition classDefinition in _mappingConfig.ClassDefinitions)
    {
      if (classDefinition.BaseClass == null 
          && (storageProviderID == null || storageProviderID == classDefinition.StorageProviderID))
      {
        baseClasses.Add (classDefinition);
      }
    }
    return baseClasses;
  }


  private ArrayList GetDerivedClassDefinitions (ClassDefinition baseClass)
  {
    //Todo: use ClassDefinition.DerivedClasses instead and inline this
    return GetDerivedClassDefinitions (baseClass, false);
  }

  private ArrayList GetDerivedClassDefinitions (ClassDefinition baseClass, bool recursive)
  {
    ArrayList derivedClasses = new ArrayList ();

    //Todo: use ClassDefinition.DerivedClasses instead
    foreach (ClassDefinition classDefinition in _mappingConfig.ClassDefinitions)
    {
      if (classDefinition.BaseClass == baseClass)
      {
        derivedClasses.Add (classDefinition);
        if (recursive)
          derivedClasses.AddRange (GetDerivedClassDefinitions (classDefinition, true));
      }
    }
    return derivedClasses;
  }

  private string[] GetDistinctStorageProviderIDs ()
  {
    ArrayList storageProviderIDs = new ArrayList();
    foreach (ClassDefinition classDefinition in _mappingConfig.ClassDefinitions)
    {
      if (!storageProviderIDs.Contains (classDefinition.StorageProviderID))
        storageProviderIDs.Add (classDefinition.StorageProviderID);
    }
    return (string[]) storageProviderIDs.ToArray (typeof (string));
  }

  private string GetDatabasename (string storageProviderID)
  {
    RdbmsProviderDefinition provider = 
        StorageProviderConfiguration.Current.StorageProviderDefinitions[storageProviderID] as RdbmsProviderDefinition;
    if (provider == null)
      return string.Empty;
    return ExtractDatabasenameFromConnectionString (provider.ConnectionString);
  }

  private string ExtractDatabasenameFromConnectionString (string connectionString)
  {
    string temp = connectionString.Substring (connectionString.IndexOf ("Initial Catalog=") + "Initial Catalog=".Length);
    string databasename = temp.Substring (0, temp.IndexOf (";"));
    return databasename;
  }
}
}
