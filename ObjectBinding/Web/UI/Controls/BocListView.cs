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

/// <summary> A BocColumnDefinitionSet is a named collection of column definitions. </summary>
[ParseChildren (true, "ColumnDefinitionCollection")]
public class BocColumnDefinitionSet
{
  /// <summary> The programmatic name of the <see cref="BocColumnDefinitionSet"/>. </summary>
  private string _setID;
  /// <summary> The displayed name of the <see cref="BocColumnDefinitionSet"/>. </summary>
  private object _title;
  /// <summary> 
  ///   The <see cref="BocColumnDefinition"/> objects stored in the 
  ///   <see cref="BocColumnDefinitionSet"/>. 
  /// </summary>
  private BocColumnDefinitionCollection _columnDefinitionCollection;
  /// <summary>
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this 
  ///   <see cref="BocColumnDefinitionSet"/> belongs. 
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> Initialize a new instance of the <see cref="BocColumnDefinitionSet"/> class. </summary>
  public BocColumnDefinitionSet()
  {
    _columnDefinitionCollection = new BocColumnDefinitionCollection (null);
  }

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinitionSet"/>.
  /// </summary>
  /// <returns> Returns the class name of the instance, followed by the <see cref="Title"/>. </returns>
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
  /// <value> 
  ///   A <see cref="string"/> providing an identifier for this <see cref="BocColumnDefinitionSet"/>.
  /// </value>
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
  /// <value> 
  ///   A <see cref="string"/> representing this <see cref="BocColumnDefinitionSet"/> on the 
  ///   rendered page.
  /// </value>
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
  ///   Gets the <see cref="BocColumnDefinition"/> objects stored in the 
  ///   <see cref="BocColumnDefinitionSet"/>. 
  /// </summary>
  /// <value>
  ///   An array of <see cref="BocColumnDefinition"/> objects that comprise this 
  ///   <see cref="BocColumnDefinitionSet"/>.
  /// </value>
  [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
  [Editor (typeof (BocSimpleColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
  [MergableProperty (false)]
  [Category ("Data")]
  [DefaultValue((string) null)]
  [NotifyParentProperty (true)]
  public BocColumnDefinitionCollection ColumnDefinitionCollection
  {
    get { return _columnDefinitionCollection; }
  }

  /// <summary> 
  ///   Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this 
  ///   <see cref="BocColumnDefinitionSet"/> belongs. </summary>
  internal IBusinessObjectBoundWebControl OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      _columnDefinitionCollection.OwnerControl = _ownerControl;
    }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "ColumnDefinitionSet"; }
  }
}

  
}
