using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Collections;

namespace Rubicon.ObjectBinding.Web.Controls
{

//  TODO: BocListItemCommand: Move long comment blocks to xml-file
/// <summary> A <see cref="BocListItemCommand"/> defines an action the user can invoke on a datarow. </summary>
[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocListItemCommand: BocCommand
{
  /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class ListItemHrefCommandInfo: Command.HrefCommandInfo
  {
    /// <summary> Initalizes a new instance </summary>
    public ListItemHrefCommandInfo()
    {
    }

    /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
    /// <value> 
    ///   The URL to link to when the rendered command is clicked. The default value is 
    ///   <see cref="String.Empty"/>. 
    /// </value>
    [Description ("The hyperlink reference of the command. Use {0} to insert the Business Object's index in the list and {1} to insert the Business Object's ID.")]
    public override string Href 
    {
      get { return base.Href; }
      set { base.Href = value; }
    }
  }

  /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class ListItemWxeFunctionCommandInfo: Command.WxeFunctionCommandInfo
  {
    /// <summary> Initalizes a new instance </summary>
    public ListItemWxeFunctionCommandInfo()
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
    ///       <term> index </term>
    ///       <description> 
    ///         The index of the <see cref="IBusinessObject"/> in the <see cref="IBusinessObjectProperty"/>.
    ///       </description>
    ///     </item>
    ///     <item>
    ///       <term> id </term>
    ///       <description> The ID, if the object is of type <see cref="IBusinessObjectWithIdentity"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> object </term>
    ///       <description> The <see cref="IBusinessObject"/> itself. </description>
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
    [Description ("A comma separated list of parameters for the command. The following reference parameter are available: index, id, object, parent, parentproperty.")]
    public override string Parameters
    {
      get { return base.Parameters; }
      set { base.Parameters = value; }
    }
  }

  /// <summary>
  ///   The <see cref="ListItemHrefCommandInfo"/> used when rendering the command as a hyperlink.
  /// </summary>
  private ListItemHrefCommandInfo _hrefCommand = new ListItemHrefCommandInfo();
  /// <summary>
  ///   The <see cref="ListItemWxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  private ListItemWxeFunctionCommandInfo _wxeFunctionCommand = new ListItemWxeFunctionCommandInfo();

  public BocListItemCommandClickEventHandler Click;

  /// <summary> Initializes an instance. </summary>
  public BocListItemCommand()
    : this (CommandType.None)
  {
  }

  /// <summary> Initializes an instance. </summary>
  public BocListItemCommand (CommandType defaultType)
    : base (defaultType)
  {
  }

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  public virtual void OnClick (
      BocCommandEnabledColumnDefinition column, 
      int listIndex, 
      IBusinessObject businessObject)
  {
    if (Click != null)
    {
      BocListItemCommandClickEventArgs e = new BocListItemCommandClickEventArgs (column, listIndex, businessObject);
      Click (this, e);
    }
  }

  /// <summary> Renders the opening tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
  /// <param name="postBackLink">
  ///   The string rendered in the <c>href</c> tag of the anchor element when the command type is
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back.
  /// </param>
  /// <param name="onClick"> 
  ///   The string rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <param name="listIndex">
  ///   An index that indentifies the <see cref="IBusinessObject"/> on which the rendered command 
  ///   is applied on.
  /// </param>
  /// <param name="businessObjectID">
  ///   An identifier for the <see cref="IBusinessObject"/> to which the rendered command is 
  ///   applied.
  /// </param>
  public void RenderBegin (
      HtmlTextWriter writer,
      string postBackLink,
      string onClick, 
      int listIndex, 
      string businessObjectID)
  {
    base.RenderBegin (writer, postBackLink, onClick, listIndex.ToString(), businessObjectID);
  }

  /// <summary>
  ///   Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommand"/>.
  /// </summary>
  /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. </param>
  /// <param name="listIndex"> 
  ///   The index of the <see cref="IBusinessObject"/> in the row where the command was clicked.
  /// </param>
  /// <param name="businessObject">
  ///   The <see cref="IBusinessObject"/> in the row where the command was clicked.
  /// </param>
  public void ExecuteWxeFunction (IWxePage wxePage, int listIndex, IBusinessObject businessObject)
  {
    if (! WxeContext.Current.IsReturningPostBack)
    {
      NameObjectCollection parameters = new NameObjectCollection();
      parameters["index"] = listIndex;
      parameters["object"] = businessObject;
      if (businessObject is IBusinessObjectWithIdentity)
        parameters["id"] = ((IBusinessObjectWithIdentity) businessObject).UniqueIdentifier;
      if (OwnerControl != null)
      {
        if (OwnerControl.DataSource != null && OwnerControl.Value != null)
          parameters["parent"] = OwnerControl.DataSource.BusinessObject;
        if (OwnerControl.Property != null)
          parameters["parentproperty"] = OwnerControl.Property;
      }
      ExecuteWxeFunction (wxePage, parameters);
    }
  }

  /// <summary> The <see cref="ListItemHrefCommandInfo"/> used when rendering the command as a hyperlink. </summary>
  /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>. </remarks>
  /// <value> A <see cref="ListItemHrefCommandInfo"/> object. </value>
  public override HrefCommandInfo HrefCommand
  {
    get { return _hrefCommand;  }
    set { _hrefCommand = (ListItemHrefCommandInfo) value; }
  }

  /// <summary>
  ///   The <see cref="ListItemWxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks> Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>. </remarks>
  /// <value> A <see cref="ListItemWxeFunctionCommandInfo"/> object. </value>
  public override WxeFunctionCommandInfo WxeFunctionCommand
  {
    get { return _wxeFunctionCommand; }
    set { _wxeFunctionCommand = (ListItemWxeFunctionCommandInfo) value; }
  }
}

/// <summary>
///   Represents the method that handles the <see cref="BocListItemCommand.Click"/> event
///   raised when clicking on a <see cref="Command"/> of type <see cref="CommandType.Event"/>.
/// </summary>
public delegate void BocListItemCommandClickEventHandler (object sender, BocListItemCommandClickEventArgs e);

/// <summary> Provides data for the <see cref="BocListItemCommand.Click"/> event. </summary>
public class BocListItemCommandClickEventArgs: BocCommandClickEventArgs
{
  private BocCommandEnabledColumnDefinition _column;
  private int _listIndex;

  /// <summary> Initializes a new instance. </summary>
  public BocListItemCommandClickEventArgs (
      BocCommandEnabledColumnDefinition column, 
      int listIndex, 
      IBusinessObject businessObject)
    : base (businessObject)
  {
    _column = column;
    _listIndex = listIndex;
  }

  /// <summary>
  ///   The <see cref="BocColumnDefinition"/> to which the command belongs.
  /// </summary>
  public BocCommandEnabledColumnDefinition Column
  {
    get { return _column; }
  }

  /// <summary>
  ///   An index that identifies the <see cref="IBusinessObject"/> on which the rendered command is applied on.
  /// </summary>
  public int ListIndex
  {
    get { return _listIndex; }
  }
}

}
