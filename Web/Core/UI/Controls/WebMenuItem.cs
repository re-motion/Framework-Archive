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
    return new MenuItem (null, null, "-", null, null, null);
  }

  string _itemID;
  string _category;
  string _text;
  string _icon;
  string _iconDisabled;

  /// <summary> The command rendered for this menu item. </summary>
  private SingleControlItemCollection _command;
  /// <summary> The control to which this object belongs. </summary>
  private Control _ownerControl;

  public MenuItem (string itemID, string category, string text, string icon, string iconDisabled, Command command)
  {
    _itemID = itemID;
    _category = category;
    _text = text;
    _icon = icon;
    _iconDisabled = iconDisabled;
    _command = new SingleControlItemCollection (command, new Type[] {typeof (Command)});
  }

  public MenuItem ()
    : this (null, null, null, null, null, new Command())
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

    if (Command.Type == CommandType.None)
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

}
