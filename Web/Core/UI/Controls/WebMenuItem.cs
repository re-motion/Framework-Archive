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
  string _itemID;
  string _category;
  string _text;
  string _icon;
  /// <summary> The <see cref="Command"/> rendered for this menu item. </summary>
  private Command _command;
  /// <summary>
  ///   The <see cref="IControl"/> to which this object belongs. 
  /// </summary>
  private Control _ownerControl;

  public MenuItem (string itemID, string category, string text, string icon, Command command)
  {
    _itemID = StringUtility.NullToEmpty (itemID);
    _category = StringUtility.NullToEmpty (category);
    _text = StringUtility.NullToEmpty (text);
    _icon = StringUtility.NullToEmpty (icon);
    _command = command;
  }

  public MenuItem ()
      : this (null, null, null, null, new Command())
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="MenuItem"/>.
  /// </summary>
  /// <returns>
  ///   Returns the <see cref="Text"/>, followed by the class name of the instance.
  /// </returns>
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
  public virtual string ItemID
  {
    get { return _itemID; }
    set { _itemID = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  public virtual string Category
  {
    get { return _category; }
    set { _category = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  public virtual string Text
  {
    get { return _text; }
    set { _text = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  public virtual string Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  /// <summary> Gets or sets the <see cref="Command"/> rendered for this menu item. </summary>
  /// <value> A <see cref="Command"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Action")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public virtual Command Command
  {
    get { return _command; }
    set 
    { 
      _command = value; 
      if (_ownerControl != null)
        _command.OwnerControl = OwnerControl;
    }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  public Command[] PersistedCommand
  {
    get
    {
      if (Command != null)
        return new Command[] {Command}; 
      else
        return new Command[0];
    }
    set 
    { 
      if (value == null || value.Length != 1 || value[0] == null)
        Command = null;
      else
        Command = value[0]; 
    }
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
        if (_command != null)
          _command.OwnerControl = value;
        OnOwnerControlChanged();
      }
    }
  }
}

}
