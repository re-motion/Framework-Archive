using System;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;

namespace Rubicon.ObjectBinding.Web.Controls
{

[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocMenuItem: MenuItem
{
  public BocMenuItem (string id, string category, string text, string icon, BocCommand command)
      : base (id, category, text, icon, command)
  {
  }

  public BocMenuItem ()
      : this (null, null, null, null, new BocCommand())
  {
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Action")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public new BocCommand Command
  {
    get { return (BocCommand) base.CommandImplementation; }
    set { base.CommandImplementation = value; }
  }

  protected override Command CommandImplementation
  {
    get { return Command; }
    set { Command = (BocCommand) value; }
  }

}

}
