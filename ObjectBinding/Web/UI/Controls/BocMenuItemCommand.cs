using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using Rubicon.Collections;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.ObjectBinding.Web.Controls
{

public class BocMenuItemCommand: BocCommand
{
  /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class MenuItemWxeFunctionCommandInfo: BocCommand.BocWxeFunctionCommandInfo
  {
    /// <summary> Create a new instance. </summary>
    public MenuItemWxeFunctionCommandInfo()
    {
    }

    /// <summary> 
    ///   Gets or sets the comma separated list of parameters passed to the WxeFunction when the rendered 
    ///   command is clicked.
    /// </summary>
    /// <remarks>
    ///   The following reference parameters can be added to the list of parameters.
    ///   <list type="table">
    ///     <listheader>
    ///       <term> Name </term>
    ///       <description> Contents </description>
    ///     </listheader>
    ///     <item>
    ///       <term> indices </term>
    ///       <description> The indices of the selected <see cref="IBusinessObject"/> instances. </description>
    ///     </item>
    ///     <item>
    ///       <term> ids </term>
    ///       <description> The IDs, if the selected objects are of type <see cref="IBusinessObjectWithIdentity"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> objects </term>
    ///       <description> The selected <see cref="IBusinessObject"/> instances themself. </description>
    ///     </item>
    ///     <item>
    ///       <term> parent </term>
    ///       <description> The containing <see cref="IBusinessObject"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> parentproperty </term>
    ///       <description> The <see cref="IBusinessObjectReferenceProperty"/> used to acess the object. </description>
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <value> 
    ///   The comma separated list of parameters passed to the WxeFunction when the rendered 
    ///   command is clicked. The default value is <see cref="String.Empty"/>. 
    /// </value>
    [Description ("A comma separated list of parameters for the command. The following reference parameter are available: indices, ids, objects, parent, parentproperty.")]
    public override string Parameters
    {
      get { return base.Parameters; }
      set { base.Parameters = value; }
    }
  }

  private bool _hasClickFired = false;
  private MenuItemWxeFunctionCommandInfo _wxeFunctionCommand;

  [Browsable (false)]
  public new WebMenuItemClickEventHandler Click;

  /// <summary> Initializes an instance. </summary>
  public BocMenuItemCommand()
    : this (CommandType.Event)
  {
  }

  /// <summary> Initializes an instance. </summary>
  public BocMenuItemCommand (CommandType defaultType)
    : base (defaultType)
  {
    _wxeFunctionCommand = new MenuItemWxeFunctionCommandInfo();
  }

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  public virtual void OnClick (BocMenuItem menuItem)
  {
    base.OnClick (null);
    if (_hasClickFired)
      return;
    _hasClickFired = true;
    if (Click != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs (menuItem);
      Click (OwnerControl, e);
    }
  }

  /// <summary>
  ///   Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommand"/>.
  /// </summary>
  /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. </param>
  /// <param name="listIndices"> 
  ///   The array of indices for the <see cref="IBusinessObject"/> instances on which the rendered 
  ///   command is applied on.
  /// </param>
  /// <param name="businessObjects"> 
  ///   The array of <see cref="IBusinessObject"/> instances on which the rendered command is applied on.
  /// </param>
  public void ExecuteWxeFunction (IWxePage wxePage, int[] listIndices, IBusinessObject[] businessObjects)
  {
    if (! WxeContext.Current.IsReturningPostBack)
    {
      NameObjectCollection parameters = PrepareWxeFunctionParameters (listIndices, businessObjects);
      ExecuteWxeFunction (wxePage, parameters);
    }
  }

  /// <summary>
  ///   Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommand"/> on a page
  ///   not implementing <see cref="IWxePage"/>.
  /// </summary>
  /// <param name="page"> The <see cref="Page"/> where this command is rendered on. </param>
  /// <param name="listIndices"> 
  ///   The array of indices for the <see cref="IBusinessObject"/> instances on which the rendered 
  ///   command is applied on.
  /// </param>
  /// <param name="businessObjects"> 
  ///   The array of <see cref="IBusinessObject"/> instances on which the rendered command is applied on.
  /// </param>
  [Obsolete ("Make public should this ever be needed.")]
  private void ExecuteWxeFunction (Page page, int[] listIndices, IBusinessObject[] businessObjects)
  {
    NameObjectCollection parameters = PrepareWxeFunctionParameters (listIndices, businessObjects);
    //ExecuteWxeFunction (page, parameters, new NameValueCollection (0));
  }

  private NameObjectCollection PrepareWxeFunctionParameters (int[] listIndices, IBusinessObject[] businessObjects)
  {
    NameObjectCollection parameters = new NameObjectCollection();
    
    parameters["indices"] = listIndices;
    parameters["objects"] = businessObjects;
    if (businessObjects.Length > 0 && businessObjects[0] is IBusinessObjectWithIdentity)
    {
      string[] ids = new string[businessObjects.Length];
      for (int i = 0; i < businessObjects.Length; i++)
        ids[i] = ((IBusinessObjectWithIdentity) businessObjects[i]).UniqueIdentifier;
      parameters["ids"] = ids;
    }
    if (OwnerControl != null)
    {
      if (OwnerControl.DataSource != null && OwnerControl.Value != null)
        parameters["parent"] = OwnerControl.DataSource.BusinessObject;
      if (OwnerControl.Property != null)
        parameters["parentproperty"] = OwnerControl.Property;
    }

    return parameters;
  }

  /// <summary>
  ///   The <see cref="MenuItemWxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>. </remarks>
  /// <value> A <see cref="MenuItemWxeFunctionCommandInfo"/> object. </value>
  public override WxeFunctionCommandInfo WxeFunctionCommand
  {
    get { return _wxeFunctionCommand; }
    set { _wxeFunctionCommand = (MenuItemWxeFunctionCommandInfo) value; }
  }
}

}
