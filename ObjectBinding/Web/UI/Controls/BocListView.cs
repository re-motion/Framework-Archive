using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using System.Collections;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary>
///   A BocColumnDefinitionSet is a named collection of column definitions.
/// </summary>
//[Designer (typeof(ColumnDefinitionSetDesigner), typeof (ControlDesigner))]
//[TypeConverter (typeof (ColumnDefinitionSetConverter))]
//[DefaultProperty ("ColumnDefinitionCollection")]
[ParseChildren (true, "ColumnDefinitionCollection")]
public class BocColumnDefinitionSet
{
  private object _title;
  private BocColumnDefinitionCollection _columnDefinitionCollection;

  private IBusinessObjectBoundWebControl _ownerControl;

  internal IBusinessObjectBoundWebControl OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      _columnDefinitionCollection.OwnerControl = _ownerControl;
    }
  }

  public BocColumnDefinitionSet (
    IBusinessObjectBoundWebControl ownerControl, 
    object title, 
    BocColumnDefinition[] columnDefinitions)
  {
    _title = title;
    _columnDefinitionCollection = new BocColumnDefinitionCollection (ownerControl);
    if (columnDefinitions != null)
      _columnDefinitionCollection.AddRange (columnDefinitions);
  }

  public BocColumnDefinitionSet (object title, BocColumnDefinition[] columnDefinitions)
    : this (null, title, null)
  {}

  public BocColumnDefinitionSet (object title)
    : this (null, title, null)
  {}

  public BocColumnDefinitionSet()
    : this (null, string.Empty, null)
  {}

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string Title
  {
    get { return (_title != null) ? _title.ToString() : string.Empty; }
    set { _title = value; }
  }

  [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
  [ListBindable (false)]
  [MergableProperty (false)]
  [DefaultValue((string) null)]
  public BocColumnDefinitionCollection ColumnDefinitionCollection
  {
    get { return _columnDefinitionCollection; }
  }
}

  
}
