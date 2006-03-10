using System;
using System.ComponentModel;
using System.Web.UI.WebControls;

using Rubicon.ObjectBinding.Web.UI.Controls;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[ToolboxItem (false)]
public class EditDetailsValidator : CustomValidator
{
  // types

  // static members and constants

  // member fields
  private Controls.BocList _owner;

  // construction and disposing

  public EditDetailsValidator (Controls.BocList owner)
  {
    _owner = owner;
  }

  // methods and properties

  protected override bool EvaluateIsValid()
  {
    return _owner.ValidateModifiableRows();
  }

  protected override bool ControlPropertiesValid()
  {
    string controlToValidate = ControlToValidate;
    if (StringUtility.IsNullOrEmpty (controlToValidate))
      return base.ControlPropertiesValid();
    else
      return NamingContainer.FindControl (controlToValidate) == _owner;
  }
}

}
