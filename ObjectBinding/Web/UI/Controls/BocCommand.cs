using System;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing.Design;
using Rubicon.Utilities;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocCommand: Command
{
  /// <summary> Initializes an instance of the <see cref="BocCommand"/> class. </summary>
  public BocCommand()
  {
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
