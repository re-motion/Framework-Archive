using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{
[TypeConverter (typeof (ExpandableObjectConverter))]
public class WebMenuItem: IControlItem
{
  public static WebMenuItem GetSeparator()
  {
    return new WebMenuItem (
        null, null, "-", null, null, WebMenuItemStyle.IconAndText, RequiredSelection.Any, false, null);
  }

  private string _itemID = "";
  private string _category = "";
  private string _text = "";
  private string _icon = "";
  private string _disabledIcon = "";
  private WebMenuItemStyle _style = WebMenuItemStyle.IconAndText;
  private RequiredSelection _requiredSelection = RequiredSelection.Any;
  private bool _isDisabled = false;

  /// <summary> The command rendered for this menu item. </summary>
  private SingleControlItemCollection _command = null;
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl = null;

  public WebMenuItem (
      string itemID, 
      string category, 
      string text, 
      string icon, 
      string disabledIcon, 
      WebMenuItemStyle style,
      RequiredSelection requiredSelection, 
      bool isDisabled,
      Command command)
  {
    _itemID = itemID;
    _category = category;
    _text = text;
    _icon = icon;
    _disabledIcon = disabledIcon;
    _style = style;
    _requiredSelection = requiredSelection;
    _isDisabled = isDisabled;
    _command = new SingleControlItemCollection (command, new Type[] {typeof (Command)});
  }

  public WebMenuItem ()
    : this (
        null, null, null, null, null, 
        WebMenuItemStyle.IconAndText, RequiredSelection.Any, false, new Command (CommandType.Event))
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary> Returns a <see cref="string"/> that represents this <see cref="WebMenuItem"/>. </summary>
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
    get { return "WebMenuItem"; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this menu item.")]
  [NotifyParentProperty (true)]
  [ParenthesizePropertyName (true)]
  [DefaultValue ("")]
  public virtual string ItemID
  {
    get { return _itemID; }
    set { _itemID = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The category to which this menu item belongs. Items of the same category will be grouped in the UI.")]
  [NotifyParentProperty (true)]
  [DefaultValue ("")]
  public virtual string Category
  {
    get { return _category; }
    set { _category = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text displayed in this menu item.")]
  [NotifyParentProperty (true)]
  [DefaultValue ("")]
  public virtual string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The URL of the icon displayed in this menu item.")]
  [NotifyParentProperty (true)]
  [DefaultValue ("")]
  public virtual string Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The URL of the icon displayed in this menu item when it is disabled. if it is not provided, the Icon's URL will be used.")]
  [NotifyParentProperty (true)]
  [DefaultValue ("")]
  public virtual string DisabledIcon
  {
    get { return _disabledIcon; }
    set { _disabledIcon = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The selection state of a connected control that is required for enabling this menu item.")]
  [NotifyParentProperty (true)]
  [DefaultValue (RequiredSelection.Any)]
  public RequiredSelection RequiredSelection
  {
    get { return _requiredSelection; }
    set { _requiredSelection = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("True to manually disable the menu item.")]
  [NotifyParentProperty (true)]
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

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The style of this menu item.")]
  [NotifyParentProperty (true)]
  [DefaultValue (WebMenuItemStyle.IconAndText)]
  public WebMenuItemStyle Style
  {
    get { return _style; }
    set { _style = value; }
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

  /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
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

public enum WebMenuItemStyle
{
  IconAndText,
  Icon,
  Text
}

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a menu item.
/// </summary>
public delegate void WebMenuItemClickEventHandler (object sender, WebMenuItemClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class WebMenuItemClickEventArgs: EventArgs
{
  /// <summary> The <see cref="WebMenuItem"/> that was clicked. </summary>
  private WebMenuItem _item;

  /// <summary> Initializes an instance. </summary>
  public WebMenuItemClickEventArgs (WebMenuItem item)
  {
    _item = item;
  }

  /// <summary> The <see cref="WebMenuItem"/> that was clicked. </summary>
  public WebMenuItem Item
  {
    get { return _item; }
  }
}

}
