using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{
[TypeConverter (typeof (ExpandableObjectConverter))]
public class MenuItem: IControlItem
{
  public static MenuItem GetSeparator()
  {
    return new MenuItem (null, null, "-", null, null, RequiredSelection.Any, false, null);
  }

  private string _itemID = "";
  private string _category = "";
  private string _text = "";
  private string _icon = "";
  private string _iconDisabled = "";
  private RequiredSelection _requiredSelection = RequiredSelection.Any;
  private bool _isDisabled = false;

  /// <summary> The command rendered for this menu item. </summary>
  private SingleControlItemCollection _command = null;
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;

  public MenuItem (
      string itemID, 
      string category, 
      string text, 
      string icon, 
      string iconDisabled, 
      RequiredSelection requiredSelection, 
      bool isDisabled,
      Command command)
  {
    _itemID = itemID;
    _category = category;
    _text = text;
    _icon = icon;
    _iconDisabled = iconDisabled;
    _requiredSelection = requiredSelection;
    _isDisabled = isDisabled;
    _command = new SingleControlItemCollection (command, new Type[] {typeof (Command)});
  }

  public MenuItem ()
    : this (null, null, null, null, null, RequiredSelection.Any, false, new Command (CommandType.Event))
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Returns a <see cref="string"/> that represents this <see cref="MenuItem"/>. </summary>
  /// <returns> Returns the <see cref="Text"/>, followed by the class name of the instance. </returns>
  public override string ToString()
  {
    string displayName = ItemID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Text;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "MenuItem"; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string ItemID
  {
    get { return _itemID; }
    set { _itemID = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string Category
  {
    get { return _category; }
    set { _category = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  public virtual string IconDisabled
  {
    get { return _iconDisabled; }
    set { _iconDisabled = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (RequiredSelection.Any)]
  public RequiredSelection RequiredSelection
  {
    get { return _requiredSelection; }
    set { _requiredSelection = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue (false)]
  public bool IsDisabled
  {
    get { return _isDisabled; }
    set { _isDisabled = value; }
  }

  /// <summary> Gets or sets the <see cref="Command"/> rendered for this menu item. </summary>
  /// <value> A <see cref="Command"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Action")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public virtual Command Command
  {
    get { return (Command) _command.Item; }
    set { _command.Item = value; }
  }

  protected bool ShouldSerializeCommand()
  {
    if (Command == null)
      return false;

    if (Command.IsDefaultType)
      return false;
    else
      return true;
  }

  /// <summary> Sets the <see cref="Command"/> to it's default value. </summary>
  /// <remarks> 
  ///   The default value is a <see cref="Command"/> object with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  protected void ResetCommand()
  {
    if (Command != null)
    {
      Command = (Command) Activator.CreateInstance (Command.GetType());
      Command.Type = CommandType.None;
    }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public SingleControlItemCollection PersistedCommand
  {
    get { return _command; }
  }

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   Does not persist <see cref="Command"/> objects with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  protected bool ShouldSerializePersistedCommand()
  {
    return ShouldSerializeCommand();
  }

  /// <summary> Gets or sets the control to which this object belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public Control OwnerControl
  {
    get { return OwnerControlImplementation;  }
    set { OwnerControlImplementation = value; }
  }

  protected virtual Control OwnerControlImplementation
  {
    get { return _ownerControl;  }
    set
    { 
      if (_ownerControl != value)
      {
        _ownerControl = value;
        if (Command != null)
          Command.OwnerControl = value;
        OnOwnerControlChanged();
      }
    }
  }
}

public enum RequiredSelection
{
  Any = 0,
  ExactlyOne = 1,
  OneOrMore = 2
}

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a menu item.
/// </summary>
public delegate void MenuItemClickEventHandler (object sender, MenuItemClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class MenuItemClickEventArgs: EventArgs
{
  /// <summary> The <see cref="MenuItem"/> that was clicked. </summary>
  private MenuItem _item;

  /// <summary> Initializes an instance. </summary>
  public MenuItemClickEventArgs (MenuItem item)
  {
    _item = item;
  }

  /// <summary> The <see cref="MenuItem"/> that was clicked. </summary>
  public MenuItem Item
  {
    get { return _item; }
  }
}

}
