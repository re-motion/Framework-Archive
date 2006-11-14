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

    // member fields

    // construction and disposing

    public TableBuilder ()
    {
    }

    // methods and properties

    protected override string SqlDataTypeBoolean { get { return "bit"; } }
    protected override string SqlDataTypeByte { get { return "tinyint"; } }
    protected override string SqlDataTypeDate { get { return "datetime"; } }
    protected override string SqlDataTypeDateTime { get { return "datetime"; } }
    protected override string SqlDataTypeDecimal { get { return "decimal (38, 3)"; } }
    protected override string SqlDataTypeDouble { get { return "float"; } }
    protected override string SqlDataTypeGuid { get { return "uniqueidentifier"; } }
    protected override string SqlDataTypeInt16 { get { return "smallint"; } }
    protected override string SqlDataTypeInt32 { get { return "int"; } }
    protected override string SqlDataTypeInt64 { get { return "bigint"; } }
    protected override string SqlDataTypeSingle { get { return "real"; } }
    protected override string SqlDataTypeString { get { return "nvarchar"; } }
    protected override string SqlDataTypeStringWithoutMaxLength { get { return "text"; } }
    protected override string SqlDataTypeBinary { get { return "image"; } }
    protected override string SqlDataTypeObjectID { get { return "uniqueidentifier"; } }
    protected override string SqlDataTypeSerializedObjectID { get { return "varchar (255)"; } }
    protected override string SqlDataTypeClassID { get { return "varchar (100)"; } }

    public override void AddToCreateTableScript (ClassDefinition classDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      createTableStringBuilder.AppendFormat ("CREATE TABLE [{0}].[{1}]\r\n"
          + "(\r\n"
          + "  [ID] uniqueidentifier NOT NULL,\r\n"
          + "  [ClassID] varchar (100) NOT NULL,\r\n"
          + "  [Timestamp] rowversion NOT NULL,\r\n\r\n"
          + "{2}  CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([ID])\r\n"
          + ")\r\n",
          SqlFileBuilder.DefaultSchema,
          classDefinition.MyEntityName,
          GetColumnList (classDefinition));
    }

    public override void AddToDropTableScript (ClassDefinition classDefinition, StringBuilder dropTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("dropTableStringBuilder", dropTableStringBuilder);

      dropTableStringBuilder.AppendFormat ("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Tables WHERE TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}')\r\n"
          + "  DROP TABLE [{1}].[{0}]\r\n",
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

      return string.Format ("  [{0}] {1}{2},\r\n{3}",
          propertyDefinition.ColumnName,
          GetSqlDataType (propertyDefinition),
          nullable,
          GetClassIDColumn (propertyDefinition));
    }

    protected override string ColumnListOfParticularClassFormatString
    {
      get { return "  -- {0} columns\r\n{1}\r\n"; }
    }
    private string GetClassIDColumn (PropertyDefinition propertyDefinition)
    {
      if (!HasClassIDColumn (propertyDefinition))
        return string.Empty;

      return string.Format ("  [{0}] {1} NULL,\r\n", RdbmsProvider.GetClassIDColumnName (propertyDefinition.ColumnName), SqlDataTypeClassID);
    }
  }
}
