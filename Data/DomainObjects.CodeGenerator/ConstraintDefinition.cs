using System;

namespace Rubicon.Data.DomainObjects.CodeGenerator
{
public class ConstraintDefinition
{
  // types

  // static members and constants

  // member fields

  private string _classname;
  private string _referencedClassname;
  private string _tablename;
  private string _columnname;
  private string _referencedTable;
  private string _referencedColumn;

  // construction and disposing

	public ConstraintDefinition (
      string classname, 
      string referencedClassname, 
      string tablename, 
      string columnname, 
      string referencedTable, 
      string referencedColumn)
	{
    _classname = classname;
    _referencedClassname = referencedClassname;
    _tablename = tablename;
    _columnname = columnname;
    _referencedTable = referencedTable;
    _referencedColumn = referencedColumn;
	}

  // methods and properties

  public string Tablename
  {
    get { return _tablename; }
  }

  public string Columnname
  {
    get { return _columnname; }
  }

  public string ReferencedTable
  {
    get { return _referencedTable; }
  }

  public string ReferencedColumn
  {
    get { return _referencedColumn; }
  }

  public string ClassName
  {
    get { return _classname; }
  }

  public string ReferencedClassName
  {
    get { return _referencedClassname; }
  }
}
}