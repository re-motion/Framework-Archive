using System;
using System.Collections;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class SqlFileBuilder : BaseBuilder
{
  // types

  // static members and constants

  private static Hashtable s_sqlTypeMapping = new Hashtable ();

  static SqlFileBuilder ()
  {
    s_sqlTypeMapping.Add ("boolean", "bit");
    s_sqlTypeMapping.Add ("byte", "tinyint");
    s_sqlTypeMapping.Add ("date", "datetime");
    s_sqlTypeMapping.Add ("dateTime", "datetime");
    s_sqlTypeMapping.Add ("decimal", "decimal");
    s_sqlTypeMapping.Add ("double", "real");
    s_sqlTypeMapping.Add ("guid", "uniqueidentifier");
    s_sqlTypeMapping.Add ("int16", "smallint");
    s_sqlTypeMapping.Add ("int32", "int");
    s_sqlTypeMapping.Add ("int64", "bigint");
    s_sqlTypeMapping.Add ("single", "float");
    s_sqlTypeMapping.Add ("string", "nvarchar");
    s_sqlTypeMapping.Add ("char", "nchar(1)");
    s_sqlTypeMapping.Add ("objectID", "uniqueidentifier");
  }

  public static string GetDBType (string mappingType, Type propertyType, NaInt32 maxLength)
  {
    if (s_sqlTypeMapping.ContainsKey (mappingType))
    {
      string typeString = (string) s_sqlTypeMapping[mappingType];
      if (mappingType == "string")
        typeString += " (" + maxLength.Value.ToString () + ")";
      return typeString;
    }
    else if (propertyType.IsEnum)
      return (string) s_sqlTypeMapping["int32"];

    throw new ArgumentException (string.Format ("Cannot map type {0} to the corresponding database type", mappingType), "mappingType");
  }

  // member fields

  private MappingConfiguration _mappingConfig;
  private StorageProviderConfiguration _storageProviderConfig;
  //TODO: eliminate this member and everything that's related to it
  private string _outputFile;

  // construction and disposing

  public SqlFileBuilder (string outputFile) : this (outputFile, null, null, null, null)
	{
  }

  public SqlFileBuilder (
      string outputFile, 
      string mappingFile, 
      string mappingSchemaFile, 
      string storageProviderFile, 
      string storageProviderSchemaFile)
          : base (outputFile)
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

  public override void Build ()
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
        bool hasChildClasses = baseClass.DerivedClasses.Count != 0;

        builder.CreateTable (baseClass.EntityName, hasChildClasses, GetColumns (baseClass));
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

  private ColumnDefinition[] GetColumns (ClassDefinition baseClass)
  {
    ArrayList columns = new ArrayList ();
    MergeColumnDefinitions (columns, baseClass);
    foreach (ClassDefinition derivedClass in GetDerivedClassDefinitions (baseClass, true))
      MergeColumnDefinitions (columns, derivedClass);

    return (ColumnDefinition[]) columns.ToArray (typeof (ColumnDefinition));
  }

  private void MergeColumnDefinitions (ArrayList columns, ClassDefinition classDefinition)
  {
    foreach (PropertyDefinition property in classDefinition.MyPropertyDefinitions)
    {
      string dataType = GetDBType (property.MappingType, property.PropertyType, property.MaxLength);
      if (property.MappingType == "objectID" && 
          GetOppositeClass (classDefinition, property.PropertyName).StorageProviderID != classDefinition.StorageProviderID)
      {
        dataType = SqlBuilder.FullObjectIdDatabaseType;
      }

      columns.Add (new ColumnDefinition (
          property.ColumnName, dataType, property.IsNullable, classDefinition.ID));

      if (property.PropertyType == typeof (ObjectID) 
          && GetOppositeClass (classDefinition, property.PropertyName).IsPartOfInheritanceHierarchy
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

  private ClassDefinitionCollection GetDerivedClassDefinitions (ClassDefinition baseClass)
  {
    return GetDerivedClassDefinitions (baseClass, false);
  }

  private ClassDefinitionCollection GetDerivedClassDefinitions (ClassDefinition baseClass, bool recursive)
  {
    ClassDefinitionCollection derivedClasses = new ClassDefinitionCollection ();

    foreach (ClassDefinition derivedClass in baseClass.DerivedClasses)
    {
      derivedClasses.Add (derivedClass);

      if (recursive)
      {
        foreach (ClassDefinition classDefinition in GetDerivedClassDefinitions (derivedClass, true))
          derivedClasses.Add (classDefinition);
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
