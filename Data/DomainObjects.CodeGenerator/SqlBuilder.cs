using System;
using System.IO;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class SqlBuilder
{
  // types

  // static members and constants

  public static readonly string ClassIdDatabaseType = "varchar (100)";
  public static readonly string FullObjectIdDatabaseType = "varchar (255)";

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
      "  CONSTRAINT [PK_%tablename%] PRIMARY KEY CLUSTERED ([ID])," + Environment.NewLine;
  private static readonly string s_tableFooter = 
      ")" + Environment.NewLine;
  private static readonly string s_alterTableHeader = 
      "ALTER TABLE [%tablename%]" + Environment.NewLine
      + "(" + Environment.NewLine;
  private static readonly string s_foreignKey = 
      "  CONSTRAINT [FK_%classname%_%refClassname%] FOREIGN KEY ([%columnname%]) REFERENCES [%refTablename%] ([%refColumnname%])" + Environment.NewLine;
  private static readonly string s_alterTableFooter = 
      ")" + Environment.NewLine;

  #endregion

  // member fields

  private TextWriter _sqlWriter;

  // construction and disposing

	public SqlBuilder (TextWriter sqlwriter, string databasename)
	{
    ArgumentUtility.CheckNotNull ("sqlwriter", sqlwriter);

    _sqlWriter = sqlwriter;

    BeginFile (databasename);
	}

  // methods and properties

  // TODO: eliminate this method and use it from BaseBuilder
  public string ReplaceTag (string original, string tag, string value)
  {
    string newString = original.Replace (tag, value);
    if (newString == original)
      throw new ApplicationException (string.Format ("ReplaceTag did not replace tag '{0}' with '{1}' in string '{2}'. Tag was not found.", tag, value, original));
    return newString;
  }

  public void AddConstraints (ConstraintDefinition[] constraints)
  {
    if (constraints.Length == 0)
      return;

    _sqlWriter.Write (ReplaceTag (s_alterTableHeader, s_tablenameTag, constraints[0].Tablename));

    foreach (ConstraintDefinition constraint in constraints)
    {
      _sqlWriter.Write (GetConstraintText (constraint));
    }

    _sqlWriter.Write (s_alterTableFooter);
    _sqlWriter.Write (s_go);
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

  public void DropTable (string tableName)
  {
    _sqlWriter.Write (ReplaceTag (s_dropTable, s_tablenameTag, tableName));
    _sqlWriter.Write (s_go);
  }

  public void CreateTable (string tableName, bool hasChildClasses, ColumnDefinition[] columnDefinitions)
  {
    _sqlWriter.Write (ReplaceTag (s_tableHeader, s_tablenameTag, tableName));

    _sqlWriter.Write (s_idColumn);

    if (hasChildClasses)
      _sqlWriter.Write (s_classIdColumn);

    _sqlWriter.Write (s_timestampColumn);

    string lastClassName = string.Empty;

    foreach (ColumnDefinition columnDefinition in columnDefinitions)
    {
      if (columnDefinition.Classname != null)
      {
        if (columnDefinition.Classname != lastClassName)
        {
          string comment = columnDefinition.Classname + " columns";
          _sqlWriter.WriteLine ();
          _sqlWriter.Write (ReplaceTag (s_columnComment, s_commentTag, comment));
          lastClassName = columnDefinition.Classname;
        }
      }
      else 
      {
        lastClassName = string.Empty;
      }
      string column = ReplaceTag (s_column, s_columnnameTag, columnDefinition.ColumnName);
      column = ReplaceTag (column, s_datatypeTag, columnDefinition.DataType);
      if (columnDefinition.IsNullable)
        column = ReplaceTag (column, s_nullableTag, "NULL");
      else
        column = ReplaceTag (column, s_nullableTag, "NOT NULL");

      _sqlWriter.Write (column);
    }

    _sqlWriter.WriteLine ();
    _sqlWriter.Write (ReplaceTag (s_primaryKey, s_tablenameTag, tableName));

    _sqlWriter.Write (s_tableFooter);
    _sqlWriter.Write (s_go);
  }

  private void BeginFile (string databasename)
  {
    _sqlWriter.Write (ReplaceTag (s_fileHeader, s_databasenameTag, databasename));
    _sqlWriter.Write (s_go);
  }
}
}
