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
  private static readonly string s_commentTag = "%comment%";
  private static readonly string s_classnameTag = "%classname%";
  private static readonly string s_refClassnameTag = "%refClassname%";
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
      "  CONSTRAINT [PK_%tablename%] PRIMARY KEY CLUSTERED ([ID])" + Environment.NewLine;
  private static readonly string s_tableFooter = 
      ")" + Environment.NewLine;
  private static readonly string s_alterTableHeader = 
      "ALTER TABLE [%tablename%]" + Environment.NewLine
      + "(" + Environment.NewLine;
  private static readonly string s_foreignKey = 
      "  CONSTRAINT [FK_%classname%_%refClassname%] FOREIGN KEY ([%columnname%]) REFERENCES [%refTablename%] ([%refColumnname%])," + Environment.NewLine;
  private static readonly string s_alterTableFooter = 
      ")" + Environment.NewLine;

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

    foreach (ClassDefinition baseClass in GetBaseClassDefinitions (_storageProviderID))
    {
      DropTable (baseClass.EntityName);
    }

    foreach (ClassDefinition baseClass in GetBaseClassDefinitions (_storageProviderID))
    {
      CreateTable (baseClass);
    }

    foreach (ClassDefinition baseClass in GetBaseClassDefinitions (_storageProviderID))
    {
      AddConstraints (baseClass);
    }

    CloseFile ();
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

  private void MergePropertyDefinitions (PropertyDefinitionCollection propertyDefinitions, ClassDefinition classDefinition)
  {
    foreach (PropertyDefinition property in classDefinition.MyPropertyDefinitions)
    {
      propertyDefinitions.Add (property);

    }
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

  private void AddConstraints (ClassDefinition baseClass)
  {
    ConstraintDefinition[] constraints = GetConstraintsRecursive (baseClass);

    if (constraints.Length == 0)
      return;

    Write (ReplaceTag (s_alterTableHeader, s_tablenameTag, constraints[0].Tablename));

    foreach (ConstraintDefinition constraint in constraints)
    {
      Write (GetConstraintText (constraint));
    }

    Write (s_alterTableFooter);
    Write (s_go);
  }

  private string GetConstraintText (ConstraintDefinition constraint)
  {
    string constraintText = s_foreignKey;
    constraintText = ReplaceTag (constraintText, s_columnnameTag, constraint.Columnname);
    constraintText = ReplaceTag (constraintText, s_refTablenameTag, constraint.ReferencedTable);
    constraintText = ReplaceTag (constraintText, s_refColumnnameTag, constraint.ReferencedColumn);
    constraintText = ReplaceTag (constraintText, s_classnameTag, constraint.ClassName);
    constraintText = ReplaceTag (constraintText, s_refClassnameTag, constraint.ReferencedClassName);
    return constraintText;
  }

  private void DropTable (string tableName)
  {
    Write (ReplaceTag (s_dropTable, s_tablenameTag, tableName));
    Write (s_go);
  }

  private void CreateTable (ClassDefinition baseClass)
  {
    Write (ReplaceTag (s_tableHeader, s_tablenameTag, baseClass.EntityName));

    Write (s_idColumn);

    if (baseClass.DerivedClasses.Count != 0)
      Write (s_classIdColumn);

    Write (s_timestampColumn);

    WriteColumns (baseClass);
    foreach (ClassDefinition derivedClass in GetDerivedClassDefinitions (baseClass, true))
      WriteColumns (derivedClass);

    WriteLine ();
    Write (ReplaceTag (s_primaryKey, s_tablenameTag, baseClass.EntityName));

    Write (s_tableFooter);
    Write (s_go);
  }

  private void WriteColumns (ClassDefinition classDefinition)
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

      WriteColumn (propertyDefinition.ColumnName, dataType, propertyDefinition.IsNullable);

      if (propertyDefinition.PropertyType == typeof (ObjectID) 
          && GetOppositeClass (classDefinition, propertyDefinition.PropertyName).IsPartOfInheritanceHierarchy
          && HasOppositeClassSameStorageProviderID (classDefinition, propertyDefinition.PropertyName))
      {
        WriteColumn (propertyDefinition.ColumnName + "ClassID", s_classIdDatabaseType, false);
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
