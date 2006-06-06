using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql
{
  public class CreateTableBuilder
  {
    // types

    // static members and constants

    private const string s_createTableFormat = "CREATE TABLE [{0}]\n"
        + "(\n"
        + "  [ID] uniqueidentifier NOT NULL,\n"
        + "  [ClassID] varchar (100) NOT NULL,\n"
        + "  [Timestamp] rowversion NOT NULL,\n\n"
        + "{1}  CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([ID])\n"
        + ")\n";

    private static Dictionary<string, string> s_sqlTypeMapping = new Dictionary<string, string> ();

    static CreateTableBuilder ()
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

      string sqlDataType = s_sqlTypeMapping[propertyDefinition.MappingTypeName];

      if (propertyDefinition.MappingTypeName == "string" && !propertyDefinition.MaxLength.IsNull)
        sqlDataType += " (" + propertyDefinition.MaxLength.ToString () + ")";

      return sqlDataType;
    }

    // member fields

    // construction and disposing

    public CreateTableBuilder ()
    {
    }

    // methods and properties

    public string GetCreateTableStatement (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      if (classDefinition.GetEntityName () == null)
        throw new ArgumentException ("The ClassDefinition must have an entity name.", "classDefinition");

      string columnList = string.Empty; 
      ClassDefinition currentClassDefinition = classDefinition;
      while (currentClassDefinition != null)
      {
        columnList = GetColumnList (currentClassDefinition, false) + columnList;

        currentClassDefinition = currentClassDefinition.BaseClass;
      }

      StringBuilder columnListStringBuilder = new StringBuilder ();
      GetColumnListOfDerivedClasses (classDefinition, columnListStringBuilder);
      columnList += columnListStringBuilder.ToString ();

      return string.Format (s_createTableFormat, classDefinition.MyEntityName, columnList);
    }

    private void GetColumnListOfDerivedClasses (ClassDefinition classDefinition, StringBuilder columnListStringBuilder)
    {
      foreach (ClassDefinition derivedClassDefinition in classDefinition.DerivedClasses)
      {
        columnListStringBuilder.Append (GetColumnList(derivedClassDefinition, true));
        GetColumnListOfDerivedClasses (derivedClassDefinition, columnListStringBuilder);
      }
    }

    public string GetColumnList (ClassDefinition classDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      StringBuilder columnListStringBuilder = new StringBuilder ();

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        columnListStringBuilder.Append (GetColumn (propertyDefinition, forceNullable));

      return string.Format ("  -- {0} columns\n{1}\n", classDefinition.ID, columnListStringBuilder);
    }

    public string GetColumn (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return GetColumn (propertyDefinition, false);
    }

    public string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

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
      RelationDefinition relationDefinition = propertyDefinition.ClassDefinition.GetRelationDefinition (propertyDefinition.PropertyName);
      if (relationDefinition != null)
      {
        IRelationEndPointDefinition oppositeEndPointDefinition = relationDefinition.GetOppositeEndPointDefinition (
            propertyDefinition.ClassDefinition.ID, propertyDefinition.PropertyName);

        if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy)
          return string.Format ("  [{0}ClassID] {1} NULL,\n", propertyDefinition.ColumnName, s_sqlTypeMapping["ClassID"]);
      }

      return null;
    }
  }
}
