using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Utilities;

namespace Rubicon.Web.UI.Controls
{
[Editor (typeof(ExpandableObjectConverter), typeof(UITypeEditor))]
public class DropDownMenuItem
{
  string _id;
  string _category;
  string _text;
  string _icon;
  /// <summary> The <see cref="Command"/> rendered for this menu item. </summary>
  private Command _command;

  public DropDownMenuItem (string id, string category, string text, string icon, Command command)
  {
    _id = StringUtility.NullToEmpty (id);
    _category = StringUtility.NullToEmpty (category);
    _text = StringUtility.NullToEmpty (text);
    _icon = StringUtility.NullToEmpty (icon);
    _command = command;
  }

  public DropDownMenuItem ()
      : this (null, null, null, null, new Command())
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
  public virtual Command Command
  {
    get { return _command; }
    set { _command = value; }
  }
}

}
