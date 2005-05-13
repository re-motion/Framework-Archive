using System;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <remarks>
///   May only be added to an <see cref="IBusinessObjectBoundWebControl"/>.
/// </remarks>
[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocMenuItem: WebMenuItem
{
  public BocMenuItem (
      string id, 
      string category, 
      string text, 
      string icon, 
      string disabledIcon, 
      RequiredSelection requiredSelection,
      bool isDisabled,
      BocMenuItemCommand command)
    : this (id, category, text, icon, disabledIcon, WebMenuItemStyle.IconAndText, requiredSelection, isDisabled, command)
  {
  }

  public BocMenuItem (
      string id, 
      string category, 
      string text, 
      string icon, 
      string disabledIcon, 
      WebMenuItemStyle style,
      RequiredSelection requiredSelection,
      bool isDisabled,
      BocMenuItemCommand command)
    : base (id, category, text, icon, disabledIcon, style, requiredSelection, isDisabled, command)
  {
  }

  public BocMenuItem ()
    : this (
        null, null, null, null, null, 
        WebMenuItemStyle.IconAndText, RequiredSelection.Any, false, new BocMenuItemCommand())
  {
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "BocMenuItem"; }
  }

  public override Command Command
  {
    get { return base.Command; }
    set { base.Command = (BocCommand) value; }
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this object belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new IBusinessObjectBoundWebControl OwnerControl
  {
    get { return (IBusinessObjectBoundWebControl) base.OwnerControlImplementation;  }
    set { base.OwnerControlImplementation = (Control) value; }
  }

  protected override Control OwnerControlImplementation
  {
    get { return (Control) OwnerControl; }
    set { OwnerControl = (IBusinessObjectBoundWebControl) value; }
  }
}

}
