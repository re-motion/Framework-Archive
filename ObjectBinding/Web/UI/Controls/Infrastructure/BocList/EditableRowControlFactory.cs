using System;

using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{
public class ModifiableRowControlFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ModifiableRowControlFactory ()
  {
  }

  // methods and properties

  public virtual IBusinessObjectBoundModifiableWebControl Create (BocSimpleColumnDefinition column, int columnIndex)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    if (columnIndex < 0) throw new ArgumentOutOfRangeException ("columnIndex");

    IBusinessObjectBoundModifiableWebControl control = column.CreateEditDetailsControl();
    IBusinessObjectProperty property = column.PropertyPath.LastProperty;

    if (control == null)
    {
      control = (IBusinessObjectBoundModifiableWebControl) ControlFactory.CreateControl (
          property, ControlFactory.EditMode.InlineEdit);
      if (control == null)
        return null;
    }

    return control;
  }

}
}
