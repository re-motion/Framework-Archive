using System;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocMenuItem: MenuItem
{
  public BocMenuItem (
      string id, 
      string category, 
      string text, 
      string icon, 
      string iconDisabled, 
      BocMenuItemCommand command)
    : base (id, category, text, icon, iconDisabled, command)
  {
  }

  public BocMenuItem ()
    : this (null, null, null, null, null, new BocMenuItemCommand())
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
