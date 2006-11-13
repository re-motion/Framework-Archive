using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer
{
  public class TableBuilder : TableBuilderBase
  {
    // types

    // static members and constants

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

    // construction and disposing

    public TableBuilder ()
    {
    }

    // methods and properties

    public override void AddToCreateTableScript (ClassDefinition classDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      createTableStringBuilder.AppendFormat ("CREATE TABLE [{0}].[{1}]\n"
          + "(\n"
          + "  [ID] uniqueidentifier NOT NULL,\n"
          + "  [ClassID] varchar (100) NOT NULL,\n"
          + "  [Timestamp] rowversion NOT NULL,\n\n"
          + "{2}  CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([ID])\n"
          + ")\n",
          SqlFileBuilder.DefaultSchema,
          classDefinition.MyEntityName,
          GetColumnList (classDefinition));
    }

    public override void AddToDropTableScript (ClassDefinition classDefinition, StringBuilder dropTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropTableStringBuilder", dropTableStringBuilder);

      dropTableStringBuilder.AppendFormat ("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\n"
          + "  DROP TABLE [{1}].[{0}]\n",
          classDefinition.MyEntityName,
          SqlFileBuilder.DefaultSchema);
    }

    public override string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable)
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

    protected override string ColumnListOfParticularClassFormatString
    {
      get { return "  -- {0} columns\n{1}\n"; }
    }
    private string GetClassIDColumn (PropertyDefinition propertyDefinition)
    {
      if (!HasClassIDColumn (propertyDefinition))
        return string.Empty;

      return string.Format ("  [{0}] {1} NULL,\n", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName), s_sqlTypeMapping["ClassID"]);
    }
  }
}
