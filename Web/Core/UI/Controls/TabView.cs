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
  [PersistChildren (true)]
  [ParseChildren (true, "LazyControls")]
  [ToolboxData ("<{0}:TabView runat=\"server\"></{0}:TabView>")]
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

    // fields

    private string _title;
    private IconInfo _icon;
    private LazyContainer _lazyContainer;

    // construction and destruction

    public TabView ()
    {
      _icon = new IconInfo ();
      _lazyContainer = new LazyContainer ();
    }

    // methods and properties

    protected override void CreateChildControls ()
    {
      base.CreateChildControls ();

      _lazyContainer.ID = ID + "_LazyContainer";
      base.Controls.Add (_lazyContainer);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [Obsolete ("Use LazyControls instead")]
    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls ();
        return base.Controls;
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [PersistenceMode (PersistenceMode.InnerProperty)]
    public ControlCollection LazyControls
    {
      get
      {
        EnsureChildControls ();
        return _lazyContainer.RealControls;
      }
    }

    public void EnsureLazyControls ()
    {
      EnsureChildControls ();
      _lazyContainer.Ensure ();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsLazyLoadingEnabled
    {
      get { return _lazyContainer.IsLazyLoadingEnabled; }
      set { _lazyContainer.IsLazyLoadingEnabled = value; }
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

    private bool ShouldSerializeIcon ()
    {
      return IconInfo.ShouldSerialize (_icon);
    }

    private void ResetIcon ()
    {
      _icon.Reset ();
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
#else
    private bool _overrideVisible = false;
    private bool _isVisible = true;

    internal void OverrideVisible ()
    {
      bool isActive = ParentMultiView.GetActiveView () == this;
      if (Visible != isActive)
      {
        _overrideVisible = true;
        Visible = isActive;
        _overrideVisible = false;
      }
    }

    public override bool Visible
    {
      get
      {
        return _isVisible;
      }
      set
      {
        if (!_overrideVisible)
          throw new InvalidOperationException ("Cannot explicitly set the visibility of a TabView.");
        _isVisible = value;
      }
    }
#endif

  }


#if NET11
  [CLSCompliant (false)]
#endif
  [ToolboxItem (false)]
  public class EmptyTabView : TabView
  {
    public EmptyTabView ()
    {
      Title = "&nbsp;";
      ID = null;
    }

    protected override ControlCollection CreateControlCollection ()
    {
      return new EmptyControlCollection (this);
    }

    protected override void CreateChildControls ()
    {
    }
  }

}
