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
//TODO: .net2.0 complier switch. Inherit from System.Web.UI.WebControls.View
[CLSCompliant (false)]
public class TabView : Rubicon.Web.UI.Controls.PageView
{
  //  constants

  // statics

  // types

  // fields
  private string _title;
  private IconInfo _icon;

  // construction and destruction

  // methods and properties
  internal void OnInsert (TabbedMultiView.MultiView multiView)
  {
    //TODO: .net2.0 complier switch. Not needed in .net2.0
    base.ParentMultiPage = multiView;
  }

  protected void OnInsert (Control multiView)
  {
    OnInsert ((TabbedMultiView.MultiView) multiView);
  }

  /// <summary> Gets or sets the title displayed in the tab for this view. </summary>
  /// <remarks> Must not be <see langword="null"/> or emtpy. </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The title displayed in this view's tab.")]
  [NotifyParentProperty (true)]
  public virtual string Title
  {
    get { return _title; }
    set
    { 
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      _title = value; 
    }
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
