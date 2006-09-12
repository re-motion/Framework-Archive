using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public class TableBuilder
  {
    // types

    // static members and constants

    public static bool IsConcreteTable (ClassDefinition classDefinition)
    {
      return classDefinition.MyEntityName != null && (classDefinition.BaseClass == null || classDefinition.BaseClass.GetEntityName () == null);
    }

    public static bool HasClassIDColumn (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      RelationDefinition relationDefinition = propertyDefinition.ClassDefinition.GetRelationDefinition (propertyDefinition.PropertyName);
      if (relationDefinition != null)
      {
        IRelationEndPointDefinition oppositeEndPointDefinition = relationDefinition.GetOppositeEndPointDefinition (
            propertyDefinition.ClassDefinition.ID, propertyDefinition.PropertyName);

        if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy
            && propertyDefinition.ClassDefinition.StorageProviderID == oppositeEndPointDefinition.ClassDefinition.StorageProviderID)
        {
          return true;
        }
      }
      return false;
    }

    private static Dictionary<string, string> s_sqlTypeMapping = new Dictionary<string, string> ();

    static TableBuilder ()
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
      s_sqlTypeMapping.Add ("string", "nvarchar");
      s_sqlTypeMapping.Add ("stringWithoutMaxLength", "text");
      s_sqlTypeMapping.Add ("binary", "image");
      s_sqlTypeMapping.Add (TypeInfo.ObjectIDMappingTypeName, "uniqueidentifier");
      s_sqlTypeMapping.Add ("SerializedObjectID", "varchar (255)");
      s_sqlTypeMapping.Add ("ClassID", "varchar (100)");
    }

    public static string GetSqlDataType (PropertyDefinition propertyDefinition)
    {
      if (!s_sqlTypeMapping.ContainsKey (propertyDefinition.MappingTypeName))
      {
        // must be an enum type
        return s_sqlTypeMapping["int32"];
      }

      if (propertyDefinition.MappingTypeName == TypeInfo.ObjectIDMappingTypeName)
      {
        ClassDefinition oppositeClass = propertyDefinition.ClassDefinition.GetOppositeClassDefinition (propertyDefinition.PropertyName);
        if (oppositeClass.StorageProviderID != propertyDefinition.ClassDefinition.StorageProviderID)
          return s_sqlTypeMapping["SerializedObjectID"];
      }

      if (propertyDefinition.MappingTypeName == "string")
      {
        if (propertyDefinition.MaxLength.IsNull)
          return s_sqlTypeMapping["stringWithoutMaxLength"];
        else
          return s_sqlTypeMapping[propertyDefinition.MappingTypeName] +  " (" + propertyDefinition.MaxLength.ToString () + ")";
      }

      return s_sqlTypeMapping[propertyDefinition.MappingTypeName];
    }

    // member fields

    private StringBuilder _createTableBuilder;
    private StringBuilder _dropTableBuilder;

    // construction and disposing

    public TableBuilder ()
    {
      _createTableBuilder = new StringBuilder ();
      _dropTableBuilder = new StringBuilder ();
    }

    // methods and properties

    public string GetCreateTableScript ()
    {
      return _createTableBuilder.ToString ();
    }

    public string GetDropTableScript ()
    {
      return _dropTableBuilder.ToString ();
    }

    public void AddTables (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddTable (currentClass);
    }

    public void AddTable (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (IsConcreteTable (classDefinition))
      {
        AddToCreateTableScript (classDefinition);
        AddToDropTableScript (classDefinition);
      }
    }

    private void AddToCreateTableScript (ClassDefinition concreteTableClassDefinition)
    {
      if (_createTableBuilder.Length != 0)
        _createTableBuilder.Append ("\n");

      _createTableBuilder.AppendFormat ("CREATE TABLE [{0}].[{1}]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "{2}  CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n",
          SqlFileBuilder.DefaultSchema,
          concreteTableClassDefinition.MyEntityName,
          GetColumnList (concreteTableClassDefinition));
    }

    private void AddToDropTableScript (ClassDefinition concreteTableClassDefinition)
    {
      if (_dropTableBuilder.Length != 0)
        _dropTableBuilder.Append ("\n");

      _dropTableBuilder.AppendFormat ("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\n"
          + "  DROP TABLE [{1}].[{0}]\n",
          concreteTableClassDefinition.MyEntityName,
          SqlFileBuilder.DefaultSchema);
    }

    private string GetColumnList (ClassDefinition concreteTableClassDefinition)
    {
      string columnList = string.Empty;
      ClassDefinition currentClassDefinition = concreteTableClassDefinition;
      while (currentClassDefinition != null)
      {
        columnList = GetColumnListOfParticularClass (currentClassDefinition, false) + columnList;

        currentClassDefinition = currentClassDefinition.BaseClass;
      }

      StringBuilder columnListStringBuilder = new StringBuilder ();
      AppendColumnListOfDerivedClasses (concreteTableClassDefinition, columnListStringBuilder);
      columnList += columnListStringBuilder.ToString ();
      return columnList;
    }

    private void AppendColumnListOfDerivedClasses (ClassDefinition classDefinition, StringBuilder columnListStringBuilder)
    {
      foreach (ClassDefinition derivedClassDefinition in classDefinition.DerivedClasses)
      {
        columnListStringBuilder.Append (GetColumnListOfParticularClass (derivedClassDefinition, true));
        AppendColumnListOfDerivedClasses (derivedClassDefinition, columnListStringBuilder);
      }
    }

    private string GetColumnListOfParticularClass (ClassDefinition classDefinition, bool forceNullable)
    {
      StringBuilder columnListStringBuilder = new StringBuilder ();

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        columnListStringBuilder.Append (GetColumn (propertyDefinition, forceNullable));

      return string.Format ("  -- {0} columns\n{1}\n", classDefinition.ID, columnListStringBuilder);
    }

    private string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      string nullable;
      if (propertyDefinition.IsNullable || forceNullable)
        nullable = " NULL";
      else
        nullable = " NOT NULL";

      return string.Format ("  [{0}] {1}{2},\n{3}",
          propertyDefinition.ColumnName,
          GetSqlDataType (propertyDefinition),
          nullable,
          GetClassIDColumn (propertyDefinition));
    }

    private string GetClassIDColumn (PropertyDefinition propertyDefinition)
    {
      if (!HasClassIDColumn (propertyDefinition))
        return string.Empty;

      return string.Format ("  [{0}] {1} NULL,\n", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName), s_sqlTypeMapping["ClassID"]);
    }
  }
}
