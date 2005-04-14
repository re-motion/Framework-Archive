using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

[ToolboxData("<{0}:TabView runat=server></{0}:TabView>")]
#if ! net20
[CLSCompliant (false)]
public class TabView : Rubicon.Web.UI.Controls.PageView
#else
public class TabView : System.Web.UI.WebControls.View
#endif
{
  //  constants

  // statics

  // types

  // fields
  private string _title;
  private IconInfo _icon;

  // construction and destruction
  public TabView()
  {
    _icon = new IconInfo();
  }

  // methods and properties
  internal void OnInsert (TabbedMultiView.MultiView multiView)
  {
#if ! net20
    base.ParentMultiPage = multiView;
#endif
  }

  protected void OnInsert (Control multiView)
  {
    OnInsert ((TabbedMultiView.MultiView) multiView);
  }

  /// <summary> Gets or sets the title displayed in the tab for this view. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The title displayed in this view's tab.")]
  [NotifyParentProperty (true)]
  public virtual string Title
  {
    get { return _title; }
    set { _title = value; }
  }

  /// <summary> Gets or sets the icon displayed in the tab for this view. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The icon displayed in this view's tab.")]
  [NotifyParentProperty (true)]
  public virtual IconInfo Icon
  {
    get { return _icon; }
    set { _icon = value; }
  }

  private bool ShouldSerializeIcon()
  {
    return IconInfo.ShouldSerialize (_icon);
  }

  private void ResetIcon()
  {
    _icon.Reset();
  }
}

}
