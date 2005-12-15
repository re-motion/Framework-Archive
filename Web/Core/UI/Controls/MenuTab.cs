using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.Utilities;
using Rubicon.Web.UI.Design;
using Rubicon.Web.UI.Globalization;

namespace Rubicon.Web.UI.Controls
{

public abstract class MenuTab: WebTab
{
  /// <summary> The command rendered for this menu item. </summary>
  private SingleControlItemCollection _command = null;

  protected MenuTab (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    Initialize();
  }

  protected MenuTab()
  {
    Initialize();
  }

  private void Initialize()
  {
    _command = new SingleControlItemCollection (new MenuTabCommand (CommandType.Href), new Type[] {typeof (MenuTabCommand)});
  }

  protected TabbedMenu TabbedMenu
  {
    get { return (TabbedMenu) OwnerControl; }
  }

  /// <summary> Gets or sets the <see cref="MenuTabCommand"/> rendered for this menu item. </summary>
  /// <value> A <see cref="MenuTabCommand"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Behavior")]
  [Description ("The command rendered for this menu item.")]
  [NotifyParentProperty (true)]
  public virtual MenuTabCommand Command
  {
    get
    {
      return (MenuTabCommand) _command.ControlItem; 
    }
    set 
    {
      _command.ControlItem = value; 
    }
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
      Command = (MenuTabCommand) Activator.CreateInstance (Command.GetType());
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

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();

    if (OwnerControl != null && ! (OwnerControl is TabbedMenu))
      throw new InvalidOperationException ("A SubMenuTab can only be added to a WebTabStrip that is part of a TabbedMenu.");

    if (Command != null)
      Command.OwnerControl = OwnerControl;
  }

  public override void LoadResources (IResourceManager resourceManager)
  {
    base.LoadResources (resourceManager);
    if (Command != null)
      Command.LoadResources (resourceManager);
  }

  public override void RenderBeginTagForCommand (HtmlTextWriter writer, bool isEnabled, WebTabStyle style)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("style", style);

    if (isEnabled && Command != null && ! IsDisabled)
    {
      NameValueCollection additionalUrlParameters = TabbedMenu.GetUrlParameters (this);
      Command.RenderBegin (
          writer, GetPostBackClientEvent(), new string[0], string.Empty, additionalUrlParameters, style);
    }
    else
    {
      style.AddAttributesToRender (writer);
      writer.RenderBeginTag (HtmlTextWriterTag.A);
    }
  }

  public override void RenderEndTagForCommand (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    if (Command != null)
      Command.RenderEnd (writer);
    else
      writer.RenderEndTag();
  }

  public override void OnClick()
  {
    base.OnClick ();
    if (! IsSelected)
      IsSelected = true;
  }
  
  public override bool EvaluateVisibile()
  {
    if (Command != null)
    {
      if (   WcagHelper.Instance.IsWaiConformanceLevelARequired()
          && Command.Type == CommandType.Event)
      {
        return false;
      }
    }
    
    return base.EvaluateVisibile();
  }

}

public class MainMenuTab: MenuTab
{
  private SubMenuTabCollection _subMenuTabs;

  public MainMenuTab (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
    _subMenuTabs = new SubMenuTabCollection (OwnerControl);
    _subMenuTabs.SetParent (this);
  }

  public MainMenuTab (string itemID, string text)
    : this (itemID, text, null)
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public MainMenuTab()
  {
    _subMenuTabs = new SubMenuTabCollection (OwnerControl);
    _subMenuTabs.SetParent (this);
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [ListBindable (false)]
  [Category ("Behavior")]
  [Description ("")]
  [DefaultValue ((string) null)]
  public SubMenuTabCollection SubMenuTabs
  {
    get { return _subMenuTabs; }
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged ();
    _subMenuTabs.OwnerControl = OwnerControl;
  }

  protected override void OnSelectionChanged()
  {
    base.OnSelectionChanged ();
    TabbedMenu.RefreshSubMenuTabStrip ();
  }
}

public class SubMenuTab: MenuTab
{
  private MainMenuTab _parent;

  public SubMenuTab (string itemID, string text, IconInfo icon)
    : base (itemID, text, icon)
  {
  }

  public SubMenuTab (string itemID, string text)
    : this (itemID, text, null)
  {
  }

  /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
  /// <exclude/>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public SubMenuTab()
  {
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public MainMenuTab Parent
  {
    get { return _parent; }
  }

  protected internal void SetParent (MainMenuTab parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);
    _parent = parent;
  }
}

[Editor (typeof (MainMenuTabCollectionEditor), typeof (UITypeEditor))]
public class MainMenuTabCollection: WebTabCollection
{
  /// <summary> Initializes a new instance. </summary>
  public MainMenuTabCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public MainMenuTabCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (SubMenuTab)})
  {
  }

  public int Add (MainMenuTab tab)
  {
    return base.Add (tab);
  }

  public void AddRange (params MainMenuTab[] tabs)
  {
    base.AddRange (tabs);
  }

  public void Insert (int index, MainMenuTab tab)
  {
    base.Insert (index, tab);
  }
}

[Editor (typeof (SubMenuTabCollectionEditor), typeof (UITypeEditor))]
public class SubMenuTabCollection: WebTabCollection
{
  private MainMenuTab _parent;

  /// <summary> Initializes a new instance. </summary>
  public SubMenuTabCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public SubMenuTabCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (SubMenuTab)})
  {
  }

  protected override void OnInsertComplete (int index, object value)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (SubMenuTab));

    base.OnInsertComplete (index, value);
    SubMenuTab tab = (SubMenuTab) value;
    tab.SetParent (_parent);
  }

  protected override void OnSetComplete(int index, object oldValue, object newValue)
  {
    ArgumentUtility.CheckNotNullAndType ("newValue", newValue, typeof (SubMenuTab));

    base.OnSetComplete (index, oldValue, newValue);
    SubMenuTab tab = (SubMenuTab) newValue;
    tab.SetParent (_parent);
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public MainMenuTab Parent
  {
    get { return _parent; }
  }

  protected internal void SetParent (MainMenuTab parent)
  {
    ArgumentUtility.CheckNotNull ("parent", parent);
    _parent = parent;
    for (int i = 0; i < InnerList.Count; i++)
      ((SubMenuTab) InnerList[i]).SetParent (_parent);
  }

  public int Add (SubMenuTab tab)
  {
    return base.Add (tab);
  }

  public void AddRange (params SubMenuTab[] tabs)
  {
    base.AddRange (tabs);
  }

  public void Insert (int index, SubMenuTab tab)
  {
    base.Insert (index, tab);
  }
}

/// <summary>
///   Represents the method that handles the <c>Click</c> event raised when clicking on a <see cref="MenuTab"/>.
/// </summary>
public delegate void MenuTabClickEventHandler (object sender, MenuTabClickEventArgs e);

/// <summary>
///   Provides data for the <c>Click</c> event.
/// </summary>
public class MenuTabClickEventArgs: WebTabClickEventArgs
{

  /// <summary> Initializes an instance. </summary>
  public MenuTabClickEventArgs (MenuTab tab)
    : base (tab)
  {
  }

  /// <summary> The <see cref="Command"/> that caused the event. </summary>
  public Command Command
  {
    get { return Tab.Command; }
  }

  /// <summary> The <see cref="MenuTab"/> that was clicked. </summary>
  public new MenuTab Tab
  {
    get { return (MenuTab) base.Tab; }
  }
}

public class MenuTabCommand: Command
{
  public MenuTabCommand ()
    : this (CommandType.Href)
  {
  }

  public MenuTabCommand (CommandType defaultType)
    : base (defaultType)
  {
  }

  /// <summary> Adds the attributes for the Wxe Function command to the anchor tag. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="postBackEvent">
  ///   The string executed upon the click on a command of types
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <param name="additionalUrlParameters">
  ///   The <see cref="NameValueCollection"/> containing additional url parameters.
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  protected override void AddAttributesToRenderForWxeFunctionCommand (
      HtmlTextWriter writer, 
      string postBackEvent,
      string onClick,
      NameValueCollection additionalUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent); 
    ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters); 
    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to AddAttributesToRenderForWxeFunctionCommand not allowed unless Type is set to CommandType.WxeFunction.");
       
    string href = "#";
    if (System.Web.HttpContext.Current != null)
      href = GetWxeFunctionPermanentUrl (additionalUrlParameters);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
    if (! StringUtility.IsNullOrEmpty (WxeFunctionCommand.Target))
      writer.AddAttribute (HtmlTextWriterAttribute.Target, WxeFunctionCommand.Target);
    if (! StringUtility.IsNullOrEmpty (onClick))
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
    if (! StringUtility.IsNullOrEmpty (ToolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, ToolTip);
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. 
  /// </summary>
  /// <param name="additionalUrlParameters">
  ///   The <see cref="NameValueCollection"/> containing additional url parameters. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  public virtual string GetWxeFunctionPermanentUrl (NameValueCollection additionalUrlParameters)
  {
    ArgumentUtility.CheckNotNull ("additionalUrlParameters", additionalUrlParameters);

    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

    Type functionType = TypeUtility.GetType (WxeFunctionCommand.TypeName, true, false);
    WxeParameterDeclaration[] parameterDeclarations = WxeFunction.GetParamaterDeclarations (functionType);
    object[] parameterValues = WxeFunction.ParseActualParameters (
        parameterDeclarations, WxeFunctionCommand.Parameters, System.Globalization.CultureInfo.InvariantCulture);   
    NameValueCollection queryString = 
        WxeFunction.SerializeParametersForQueryString (parameterDeclarations, parameterValues);
    queryString.Add (additionalUrlParameters);
    return WxeContext.GetPermanentUrl (HttpContext.Current, functionType, queryString);
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. 
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  public string GetWxeFunctionPermanentUrl ()
  {
    return GetWxeFunctionPermanentUrl (new NameValueCollection (0));
  }
}
}
