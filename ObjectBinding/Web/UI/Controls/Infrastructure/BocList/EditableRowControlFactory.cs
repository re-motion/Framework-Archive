using System;

using Rubicon.ObjectBinding.Web;
using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

public class EditableRowControlFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public EditableRowControlFactory ()
  {
  }

  // methods and properties

  public virtual IBusinessObjectBoundEditableWebControl Create (BocSimpleColumnDefinition column, int columnIndex)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    if (columnIndex < 0) throw new ArgumentOutOfRangeException ("columnIndex");

    IBusinessObjectBoundEditableWebControl control = column.CreateEditDetailsControl();

    if (control == null)
    {
      control = (IBusinessObjectBoundEditableWebControl) ControlFactory.CreateControl (
          column.PropertyPath.LastProperty, ControlFactory.EditMode.InlineEdit);
    }

    return control;
  }
}

}
