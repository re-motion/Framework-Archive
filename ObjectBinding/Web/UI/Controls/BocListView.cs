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
  /// <summary> The displayed name of the set. </summary>
  private object _title;
  /// <summary> The <see cref="BocColumnDefinition"/> objects stored in the set. </summary>
  private BocColumnDefinitionCollection _columnDefinitionCollection;
  /// <summary> The <see cref="IBusinessObjectBoundWebControl"/> to which this set belongs to. </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> Simple Constructor. </summary>
  /// <param name="ownerControl">
  ///   The control this <see cref="BocColumnDefinitionSet"/> belongs to.
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
    _columnDefinitionCollection = new BocColumnDefinitionCollection (
      ownerControl, 
      new Type[] {typeof (BocSimpleColumnDefinition)});
    
    if (columnDefinitions != null)
      _columnDefinitionCollection.AddRange (columnDefinitions);
  }

  /// <summary> Simple Constructor. </summary>
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
  {}

  /// <summary> Simple Constructor. </summary>
  public BocColumnDefinitionSet()
    : this (null, string.Empty, null)
  {}

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinitionSet"/>.
  /// </summary>
  /// <returns>
  ///   Returns the class name of the instance, followed by the <see cref="Title"/>.
  /// </returns>
  public override string ToString()
  {
    if (StringUtility.IsNullOrEmpty (Title))
      return GetType().Name;
    else
      return string.Format ("{0} ({1})", GetType().Name, Title);
  }

  /// <summary> The displayed name of the set. </summary>
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

  /// <summary> The <see cref="BocColumnDefinition"/> objects stored in the set. </summary>
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

  /// <summary> The <see cref="IBusinessObjectBoundWebControl"/> to which this set belongs to. </summary>
  internal IBusinessObjectBoundWebControl OwnerControl
  {
    get { return _ownerControl; }
    set 
    {
      _ownerControl = value; 
      _columnDefinitionCollection.OwnerControl = _ownerControl;
    }
  }
}

  
}
