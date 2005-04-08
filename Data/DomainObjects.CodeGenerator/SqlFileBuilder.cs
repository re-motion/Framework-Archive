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
public class SqlFileBuilder : FileBuilder
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
    s_sqlTypeMapping.Add ("decimal", "decimal (38, 3)");
    s_sqlTypeMapping.Add ("double", "float");
    s_sqlTypeMapping.Add ("guid", "uniqueidentifier");
    s_sqlTypeMapping.Add ("int16", "smallint");
    s_sqlTypeMapping.Add ("int32", "int");
    s_sqlTypeMapping.Add ("int64", "bigint");
    s_sqlTypeMapping.Add ("single", "real");
//TODO: make usage of nvarchar/varchar for string properties configurable (app.config)
    s_sqlTypeMapping.Add ("string", "nvarchar");
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

    throw new ArgumentException (string.Format ("Cannot map type {0} to the corresponding database type.", mappingType), "mappingType");
  }

  private static readonly string s_classIdDatabaseType = "varchar (100)";
  private static readonly string s_fullObjectIdDatabaseType = "varchar (255)";

  #region Tags

  private static readonly string s_databasenameTag = "%databasename%";
  private static readonly string s_tablenameTag = "%tablename%";
  private static readonly string s_columnnameTag = "%columnname%";
  private static readonly string s_datatypeTag = "%datatype%";
  private static readonly string s_nullableTag = "%nullable%";
  private static readonly string s_constraintnameTag = "%constraintname%";
  private static readonly string s_commentTag = "%comment%";
  private static readonly string s_refTablenameTag = "%refTablename%";
  private static readonly string s_refColumnnameTag = "%refColumnname%";

  #endregion

  #region Templates

  private static readonly string s_fileHeader = 
      "USE %databasename%" + Environment.NewLine;
  private static readonly string s_go = 
      "GO" + Environment.NewLine
      + "" + Environment.NewLine;
  private static readonly string s_dropTable = 
      "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '%tablename%')" + Environment.NewLine
      + "DROP TABLE [%tablename%]" + Environment.NewLine;
  private static readonly string s_dropConstraints = 
      "-- Drop foreign keys of all tables that will be created below" + Environment.NewLine
      + "DECLARE @statement nvarchar (4000)" + Environment.NewLine
      + "SET @statement = ''" + Environment.NewLine
      + "SELECT @statement = @statement + 'ALTER TABLE [' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' " + Environment.NewLine
      + "    FROM sysobjects fk INNER JOIN sysobjects t ON fk.parent_obj = t.id " + Environment.NewLine
      + "    WHERE fk.xtype = 'F' AND t.name IN (%tablename%)" + Environment.NewLine
      + "    ORDER BY t.name, fk.name" + Environment.NewLine
      + "exec sp_executesql @statement" + Environment.NewLine;
  private static readonly string s_tableHeader = 
      "CREATE TABLE [%tablename%]" + Environment.NewLine
      + "(" + Environment.NewLine;
  private static readonly string s_timestampColumn = 
    "  [Timestamp] rowversion NOT NULL," + Environment.NewLine;
  private static readonly string s_idColumn = 
    "  [ID] uniqueidentifier NOT NULL," + Environment.NewLine;
  private static readonly string s_classIdColumn = 
    "  [ClassID] varchar (100) NOT NULL," + Environment.NewLine;
  private static readonly string s_column = 
    "  [%columnname%] %datatype% %nullable%," + Environment.NewLine;
  private static readonly string s_columnComment = 
      "  -- %comment%" + Environment.NewLine;
  private static readonly string s_primaryKey = 
      "  CONSTRAINT [PK_%constraintname%] PRIMARY KEY CLUSTERED ([ID])" + Environment.NewLine;
  private static readonly string s_tableFooter = 
      ")" + Environment.NewLine;
  private static readonly string s_alterTableHeader = 
      "ALTER TABLE [%tablename%] ADD" + Environment.NewLine;
  private static readonly string s_foreignKey = 
      "  CONSTRAINT [FK_%constraintname%] FOREIGN KEY ([%columnname%]) REFERENCES [%refTablename%] ([%refColumnname%])";
  private static readonly string s_foreignKeySeparator = 
      "," + Environment.NewLine;
  private static readonly string s_alterTableFooter = 
      "" + Environment.NewLine;

  #endregion

  // member fields

  private string _storageProviderID;

  // construction and disposing

  public SqlFileBuilder (string outputFile, string storageProviderID)
      : base (outputFile)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

    _storageProviderID = storageProviderID;
  }

  // methods and properties

  public override void Build ()
  {
    OpenFile ();
    BeginFile (GetDatabasename (_storageProviderID));

    WriteDropConstraints (_storageProviderID);

    foreach (ClassDefinition baseClass in GetBaseClassDefinitions (_storageProviderID))
    {
      WriteDropTable (baseClass.EntityName);
    }

    foreach (ClassDefinition baseClass in GetBaseClassDefinitions (_storageProviderID))
    {
      WriteCreateTable (baseClass);
    }

    foreach (ClassDefinition baseClass in GetBaseClassDefinitions (_storageProviderID))
    {
      WriteConstraints (baseClass);
    }

    CloseFile ();
  }

  private RelationEndPointDefinition[] GetRelationEndPointsWithForeignKey (ClassDefinition classDefinition)
  {
    ArrayList relationEndPoints = new ArrayList ();
    foreach (IRelationEndPointDefinition endPoint in classDefinition.GetMyRelationEndPointDefinitions ())
    {
      RelationEndPointDefinition relationEndPoint = endPoint as RelationEndPointDefinition;
      if (relationEndPoint != null)
      {
        if (HasOppositeClassSameStorageProviderID (relationEndPoint.ClassDefinition, relationEndPoint.PropertyName))
          relationEndPoints.Add (relationEndPoint);
      }
    }
    return (RelationEndPointDefinition[]) relationEndPoints.ToArray (typeof (RelationEndPointDefinition));
  }

  private RelationEndPointDefinition[] GetRelationEndPointsWithForeignKeyRecursive (ClassDefinition baseClass)
  {
    ArrayList relationEndPoints = new ArrayList ();
    relationEndPoints.AddRange (GetRelationEndPointsWithForeignKey (baseClass));

    foreach (ClassDefinition classDefinition in GetDerivedClassDefinitions (baseClass))
    {
      relationEndPoints.AddRange (GetRelationEndPointsWithForeignKey (classDefinition));
    }
    return (RelationEndPointDefinition[]) relationEndPoints.ToArray (typeof (RelationEndPointDefinition));
  }

  private ClassDefinition GetOppositeClass (ClassDefinition classDefinition, string propertyName)
  {
    return classDefinition.GetOppositeEndPointDefinition (propertyName).ClassDefinition;;
  }

  private ArrayList GetBaseClassDefinitions (string storageProviderID)
  {
    ArrayList baseClasses = new ArrayList ();
    foreach (ClassDefinition classDefinition in MappingConfiguration.Current.ClassDefinitions)
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
    ClassDefinitionCollection derivedClasses = new ClassDefinitionCollection ();

    foreach (ClassDefinition derivedClass in baseClass.DerivedClasses)
    {
      derivedClasses.Add (derivedClass);

      foreach (ClassDefinition classDefinition in GetDerivedClassDefinitions (derivedClass))
        derivedClasses.Add (classDefinition);
    }

    return derivedClasses;
  }

  private string GetDatabasename (string storageProviderID)
  {
    RdbmsProviderDefinition provider = 
        StorageProviderConfiguration.Current.StorageProviderDefinitions[storageProviderID] as RdbmsProviderDefinition;
    if (provider == null)
      return string.Empty;
    return GetDatabasenameFromConnectionString (provider.ConnectionString);
  }

  private string GetDatabasenameFromConnectionString (string connectionString)
  {
    string temp = connectionString.Substring (connectionString.IndexOf ("Initial Catalog=") + "Initial Catalog=".Length);
    string databasename = temp.Substring (0, temp.IndexOf (";"));
    return databasename;
  }

  private void WriteConstraints (ClassDefinition classDefinition)
  {
    RelationEndPointDefinition[] endPoints = GetRelationEndPointsWithForeignKeyRecursive (classDefinition);
    if (endPoints.Length == 0)
      return;

    Write (ReplaceTag (s_alterTableHeader, s_tablenameTag, classDefinition.EntityName));

    bool constraintWritten = false;
    foreach (RelationEndPointDefinition endPoint in endPoints)
    {
      if (constraintWritten)
      {
        Write (s_foreignKeySeparator);
      }
      WriteConstraint (endPoint);
      constraintWritten = true;
    }

    Write (s_alterTableFooter);
    Write (s_go);
  }

  private void WriteConstraint (RelationEndPointDefinition endPoint)
  {
    string constraintText = s_foreignKey;
    constraintText = ReplaceTag (constraintText, s_columnnameTag, endPoint.PropertyDefinition.ColumnName);
    constraintText = ReplaceTag (constraintText, s_refTablenameTag, GetOppositeClass (endPoint.ClassDefinition, endPoint.PropertyName).EntityName);
    constraintText = ReplaceTag (constraintText, s_refColumnnameTag, "ID");
    constraintText = ReplaceTag (constraintText, s_constraintnameTag, endPoint.ClassDefinition.GetRelationDefinition(endPoint.PropertyName).ID);
    Write (constraintText);
  }

  private void WriteDropConstraints (string storageProviderID)
  {
    string tableNames = null;
    foreach (ClassDefinition classDefinition in GetBaseClassDefinitions (storageProviderID))
    {
      if (tableNames != null)
        tableNames += ", '" + classDefinition.EntityName + "'";
      else
        tableNames = "'" + classDefinition.EntityName + "'";
    }

    Write (ReplaceTag (s_dropConstraints, s_tablenameTag, tableNames));
    Write (s_go);
  }

  private void WriteDropTable (string tableName)
  {
    Write (ReplaceTag (s_dropTable, s_tablenameTag, tableName));
    Write (s_go);
  }
  
  private void WriteCreateTable (ClassDefinition baseClass)
  {
    Write (ReplaceTag (s_tableHeader, s_tablenameTag, baseClass.EntityName));

    Write (s_idColumn);

    Write (s_classIdColumn);

    Write (s_timestampColumn);

    WriteColumns (baseClass, false);
    foreach (ClassDefinition derivedClass in GetDerivedClassDefinitions (baseClass))
      WriteColumns (derivedClass, true);

    WriteLine ();
    Write (ReplaceTag (s_primaryKey, s_constraintnameTag, baseClass.EntityName));

    Write (s_tableFooter);
    Write (s_go);
  }

  private void WriteColumns (ClassDefinition classDefinition, bool allColumnsNullable)
  {
    string comment = classDefinition.ID + " columns";
    WriteLine ();
    Write (ReplaceTag (s_columnComment, s_commentTag, comment));

    foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
    {
      string dataType;
      if (propertyDefinition.MappingType == "objectID" && !HasOppositeClassSameStorageProviderID (classDefinition, propertyDefinition.PropertyName))
        dataType = s_fullObjectIdDatabaseType;
      else
        dataType = GetDBType (propertyDefinition.MappingType, propertyDefinition.PropertyType, propertyDefinition.MaxLength);

      WriteColumn (propertyDefinition.ColumnName, dataType, allColumnsNullable || propertyDefinition.IsNullable);

      if (propertyDefinition.PropertyType == typeof (ObjectID) 
          && GetOppositeClass (classDefinition, propertyDefinition.PropertyName).IsPartOfInheritanceHierarchy
          && HasOppositeClassSameStorageProviderID (classDefinition, propertyDefinition.PropertyName))
      {
        WriteColumn (propertyDefinition.ColumnName + "ClassID", s_classIdDatabaseType, allColumnsNullable || propertyDefinition.IsNullable);
      }
    }
  }

  private void WriteColumn (string columnName, string dataType, bool isNullable)
  {
    string column = s_column;
    column = ReplaceTag (column, s_columnnameTag, columnName);

    column = ReplaceTag (column, s_datatypeTag, dataType);

    if (isNullable)
      column = ReplaceTag (column, s_nullableTag, "NULL");
    else
      column = ReplaceTag (column, s_nullableTag, "NOT NULL");

    Write (column);
  }

  private bool HasOppositeClassSameStorageProviderID (ClassDefinition classDefinition, string propertyName)
  {
    return GetOppositeClass (classDefinition, propertyName).StorageProviderID == classDefinition.StorageProviderID;
  }

  private void BeginFile (string databasename)
  {
    Write (ReplaceTag (s_fileHeader, s_databasenameTag, databasename));
    Write (s_go);
  }
}
}
