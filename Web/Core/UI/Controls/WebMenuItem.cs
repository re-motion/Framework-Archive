using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{
[TypeConverter (typeof (ExpandableObjectConverter))]
public class MenuItem
{
  string _id;
  string _category;
  string _text;
  string _icon;
  /// <summary> The <see cref="Command"/> rendered for this menu item. </summary>
  private Command _command;
  /// <summary>
  ///   The <see cref="IControl"/> to which this <see cref="BocColumnDefinition"/> belongs. 
  /// </summary>
  private IControl _ownerControl;

  public MenuItem (string id, string category, string text, string icon, Command command)
  {
    _id = StringUtility.NullToEmpty (id);
    _category = StringUtility.NullToEmpty (category);
    _text = StringUtility.NullToEmpty (text);
    _icon = StringUtility.NullToEmpty (icon);
    _command = command;
  }

  public MenuItem ()
      : this (null, null, null, null, new Command())
  {
  }

  protected virtual void OnCommandChanging (Command currentCommand, Command newCommand)
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  public virtual string ID
  {
    get { return _id; }
    set { _id = value; }
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
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Action")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public Command Command
  {
    get { return CommandImplementation; }
    set { CommandImplementation = value; }
  }

  protected virtual Command CommandImplementation
  {
    get { return _command; }
    set 
    { 
      _command = value; 
      if (_ownerControl != null)
        _command.OwnerControl = _ownerControl;
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IControl"/> to which this <see cref="BocColumnDefinition"/> belongs. 
  /// </summary>
  protected internal IControl OwnerControl
  {
    get
    {
      return _ownerControl; 
    }
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
