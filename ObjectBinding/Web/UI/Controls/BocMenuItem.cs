using System;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI;

namespace Rubicon.ObjectBinding.Web.Controls
{

public interface IBocMenuItemContainer
{
  bool IsReadOnly { get; }
  bool IsSelectionEnabled { get; }
  IBusinessObject[] GetSelectedBusinessObjects();
  void RemoveBusinessObjects (IBusinessObject[] businessObjects);
  void InsertBusinessObjects (IBusinessObject[] businessObjects);
}

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

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged ();
    ArgumentUtility.CheckNotNullAndType ("OwnerControl", OwnerControl, typeof (IBocMenuItemContainer));
  }

  protected IBocMenuItemContainer BocMenuItemContainer
  {
    get { return (IBocMenuItemContainer) OwnerControl; }
  }

  protected override void PreRender()
  {
    base.PreRender();

    IsDisabled = EvaluateDisabled ();
    IsVisible = EvaluateVisible ();
  }

  protected virtual bool EvaluateVisible ()
  {
    if (! IsVisible)
      return false;

    bool isReadOnly = BocMenuItemContainer.IsReadOnly;
    bool isSelectionEnabled = BocMenuItemContainer.IsSelectionEnabled;

    if (Command != null)
    {
      if (! isReadOnly && Command.Show == CommandShow.ReadOnly)
        return false;
      if (isReadOnly && Command.Show == CommandShow.EditMode)
        return false;
    }
    bool isSelectionRequired =   RequiredSelection == RequiredSelection.ExactlyOne 
                              || RequiredSelection == RequiredSelection.OneOrMore; 
    if (!isSelectionEnabled && isSelectionRequired)
      return false;

    return true;
  }

  protected virtual bool EvaluateDisabled ()
  {
    return IsDisabled;
  }
}

}
