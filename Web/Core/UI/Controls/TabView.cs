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

  [DefaultProperty ("RealControls")]
  [ToolboxData("<{0}:TabView runat=\"server\"></{0}:TabView>")]
#if NET11
  [CLSCompliant (false)]
  public class TabView : Microsoft.Web.UI.WebControls.PageView
#else
  public class TabView : System.Web.UI.WebControls.View
#endif
  {
    //  constants

    // statics

    // types
    protected override void AddedControl(Control control, int index)
    {
      base.AddedControl (control, index);
    }

    // fields
    private string _title;
    private IconInfo _icon;
    private bool _isEnsured = false;

    // construction and destruction
    public TabView()
    {
      _icon = new IconInfo();
    }

    // methods and properties

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public ControlCollection RealControls
    {
      get
      {
        return base.Controls;
      }
    }

    internal TabbedMultiView.MultiView ParentMultiView
    {
      get 
      {
        return (TabbedMultiView.MultiView) Parent;
      }
    }

    internal void OnInsert (TabbedMultiView.MultiView multiView)
    {
#if NET11
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

#if NET11
    protected override Microsoft.Web.UI.WebControls.RenderPathID RenderPath
    {
      get 
      { 
    	  if (this.IsDesignMode)
		      return Microsoft.Web.UI.WebControls.RenderPathID.DesignerPath;
        else
	  	    return Microsoft.Web.UI.WebControls.RenderPathID.DownLevelPath;
      }
    }

    public bool Active
    {
      get { return Selected; }
    }
#endif

  }


#if NET11
  [CLSCompliant (false)]
#endif
  [ToolboxItem (false)]
  public class EmptyTabView: TabView
  {
    public EmptyTabView()
    {
      Title = "&nbsp;";
      ID = null;
    }

    protected override ControlCollection CreateControlCollection()
    {
      return new EmptyControlCollection (this);
    }
  }

}
