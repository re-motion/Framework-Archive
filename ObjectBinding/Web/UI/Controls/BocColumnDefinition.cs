using System;
using System.Web.UI.WebControls;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   A ColumnDefinitionCollection is a named collection of column definitions.
/// </summary>
public class ColumnDefinitionCollection
{
  private object _title;
  private ColumnDefinition[] _columns;

  public ColumnDefinitionCollection (object title, ColumnDefinition[] columns)
  {
    ArgumentUtility.CheckNotNull ("columns", columns);
    _title = title;
    _columns = columns;
  }

  public string Title
  {
    get { return _title.ToString(); }
  }

  public ColumnDefinition[] Columns
  {
    get { return _columns; }
  }
}

/// <summary>
///   A ColumnDefinition defines how to display a column of a list. 
/// </summary>
public abstract class ColumnDefinition
{
  private Unit _width; 

  public ColumnDefinition (Unit width)
  {
    _width = width;
  }

  public abstract string Title { get; }

  public Unit Width 
  { 
    get { return _width; } 
  }
}

/// <summary>
///   A column definition containing no data, but an <see cref="ItemCommand"/>.
/// </summary>
public class CommandColumnDefinition: ColumnDefinition
{
  private string _title;
  private ItemCommand _command;
  
  public CommandColumnDefinition (ItemCommand command, string title, Unit width)
    : base (width)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    _command = command;
    _title = StringUtility.NullToEmpty (title);
  }

  public override string Title
  {
    get { return _title;  }
  }
}

/// <summary>
///   A column definition for displaying data.
/// </summary>
public abstract class ValueColumnDefinition: ColumnDefinition
{
  public ValueColumnDefinition (Unit width)
    : base (width)
  {
  }

  public abstract string GetStringValue (IBusinessObject obj);
}

/// <summary>
///   A column definition for displaying a single property path.
/// </summary>
/// <remarks>
///   Note that using the methods of <see cref="BusinessObjectPropertyPath"/>, the original value of this property can be retreived or changed.
/// </remarks>
public class SimpleColumnDefinition: ValueColumnDefinition
{
  private BusinessObjectPropertyPath _propertyPath;
  private string _title;

  public SimpleColumnDefinition (BusinessObjectPropertyPath propertyPath, string title, Unit width)
    : base (width)
  {
    _propertyPath = propertyPath;
    _title = title;
  }

  public BusinessObjectPropertyPath PropertyPath 
  { 
    get { return _propertyPath; }
  }

  public override string Title
  {
    get { return (_title == null) ? _propertyPath.LastProperty.DisplayName : _title;  }
  }

  public override string GetStringValue (IBusinessObject obj)
  {
    return PropertyPath.GetStringValue (obj);
  }
}

/// <summary>
///   A column definition for displaying a string made up from different property paths.
/// </summary>
/// <remarks>
///   Note that values in these columns can usually not be modified directly.
/// </remarks>
public class CompoundColumnDefinition: ValueColumnDefinition
{
  private string _title;
  private string _formatString;
  private BusinessObjectPropertyPath[] _propertyPaths;

  public CompoundColumnDefinition (string formatString, BusinessObjectPropertyPath[] propertyPaths, string title, Unit width)
    : base (width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPaths", propertyPaths);
    ArgumentUtility.CheckNotNullOrEmpty ("title", title);
    _title = title;
    _formatString = formatString;
    _propertyPaths = propertyPaths;
  }

  public override string GetStringValue(IBusinessObject obj)
  {
    string[] strings = new string[_propertyPaths.Length];
    for (int i = 0; i < _propertyPaths.Length; ++i)
      strings[i] = _propertyPaths[i].GetStringValue (obj);

    return String.Format (_formatString, strings);
  }

  public override string Title
  {
    get { return _title; }
  }
}

}
