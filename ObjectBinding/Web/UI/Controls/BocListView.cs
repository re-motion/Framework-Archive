using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A BocColumnDefinitionSet is a named collection of column definitions. </summary>
[ParseChildren (true, "ColumnDefinitionCollection")]
public class BocColumnDefinitionSet: BusinessObjectControlItem
{
  private string _setID;
  private object _title;
  /// <summary> 
  ///   The <see cref="BocColumnDefinition"/> objects stored in the <see cref="BocColumnDefinitionSet"/>. 
  /// </summary>
  private BocColumnDefinitionCollection _columnDefinitions;

  /// <summary> 
  ///   Initialize a new instance of the <see cref="BocColumnDefinitionSet"/> class 
  ///   with the <see cref="IBusinessObjectBoundWebControl"/> to which it belongs, a title,
  ///   and an array of <see cref="BocColumnDefinition"/> objects. 
  /// </summary>
  /// <param name="ownerControl">
  ///   The <see cref="IBusinessObjectBoundWebControl"/> this <see cref="BocColumnDefinitionSet"/> 
  ///   belongs.
  /// </param>
  /// <param name="title">
  ///   The <see cref="string"/> symbolizing this <see cref="BocColumnDefinitionSet"/> 
  ///   on the rendered page.
  /// </param>
  /// <param name="columnDefinitions">
  ///   An array of <see cref="BocColumnDefinition"/> objects that comprise this 
  ///   <see cref="BocColumnDefinitionSet"/>.
  /// </param>
  public BocColumnDefinitionSet (
      IBusinessObjectBoundWebControl ownerControl, 
      object title, 
      BocColumnDefinition[] columnDefinitions)
  {
    _title = title;
    _columnDefinitions = new BocColumnDefinitionCollection (
      ownerControl);
    
    if (columnDefinitions != null)
      _columnDefinitions.AddRange (columnDefinitions);
  }

  /// <summary> 
  ///   Initialize a new instance of the <see cref="BocColumnDefinitionSet"/> class with a title
  ///   and an array of <see cref="BocColumnDefinition"/> objects. 
  /// </summary>
  /// <param name="title">
  ///   The <see cref="string"/> representing this <see cref="BocColumnDefinitionSet"/> 
  ///   on the rendered page.
  /// </param>
  /// <param name="columnDefinitions">
  ///   An array of <see cref="BocColumnDefinition"/> objects that comprise this 
  ///   <see cref="BocColumnDefinitionSet"/>.
  /// </param>
  public BocColumnDefinitionSet (object title, BocColumnDefinition[] columnDefinitions)
    : this (null, title, columnDefinitions)
  {
  }

  /// <summary> Initialize a new instance of the <see cref="BocColumnDefinitionSet"/> class. </summary>
  public BocColumnDefinitionSet()
    : this (null, string.Empty, null)
  {
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();
    _columnDefinitions.OwnerControl = OwnerControl;
  }

  public override string ToString()
  {
    string displayName = SetID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Title;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the programmatic name of the <see cref="BocColumnDefinitionSet"/>. </summary>
  /// <value> A <see cref="string"/> providing an identifier for this <see cref="BocColumnDefinitionSet"/>. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this column definition set.")]
  [Category ("Misc")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string SetID
  {
    get { return _setID; }
    set { _setID = value; }
  }

  /// <summary> Gets or sets the displayed name of the <see cref="BocColumnDefinitionSet"/>. </summary>
  /// <value> A <see cref="string"/> representing this <see cref="BocColumnDefinitionSet"/> on the rendered page. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string Title
  {
    get { return (_title != null) ? _title.ToString() : string.Empty; }
    set { _title = value; }
  }

  /// <summary> 
  ///   Gets the <see cref="BocColumnDefinition"/> objects stored in the <see cref="BocColumnDefinitionSet"/>.  
  /// </summary>
  /// <value>
  ///   An array of <see cref="BocColumnDefinition"/> objects that comprise this <see cref="BocColumnDefinitionSet"/>.
  /// </value>
  [Editor (typeof (BocSimpleColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
  [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
  [Category ("Data")]
  [DefaultValue((string) null)]
  [NotifyParentProperty (true)]
  public BocColumnDefinitionCollection ColumnDefinitions
  {
    get { return _columnDefinitions; }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "ColumnDefinitionSet"; }
  }
}

  
}
