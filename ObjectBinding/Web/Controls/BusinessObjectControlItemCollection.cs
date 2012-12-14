using System;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{
 
/// <summary> A collection of <see cref="BusinessObjectControlItem"/> objects. </summary>
public abstract class BusinessObjectControlItemCollection : ControlItemCollection
{
  /// <summary> Initializes a new instance. </summary>
  public BusinessObjectControlItemCollection (IBusinessObjectBoundWebControl ownerControl, Type[] supportedTypes)
    : base ((Control) ownerControl, supportedTypes)
  {
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public new IBusinessObjectBoundWebControl OwnerControl
  {
    get { return (IBusinessObjectBoundWebControl) base.OwnerControl; }
    set { base.OwnerControl = (Control) value; }
  }
}

public abstract class BusinessObjectControlItem: IControlItem
{
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> to which this item belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectBoundWebControl OwnerControl
  {
    get { return _ownerControl; }
    set
    {
      if (_ownerControl != value)
      {
        _ownerControl = value;
        OnOwnerControlChanged();
      }
    }
  }

  Control IControlItem.OwnerControl
  {
    get { return (Control) _ownerControl; }
    set { OwnerControl = (IBusinessObjectBoundWebControl) value; }
  }
}

}
