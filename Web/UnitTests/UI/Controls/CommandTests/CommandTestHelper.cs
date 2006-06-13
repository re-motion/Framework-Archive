using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  public class CommandTestHelper
  {
    // types

    // static members

    // member fields

    private Type _functionType;
    private string _functionTypeName;
    private string _wxeFunctionParameters = "\"Value1\"";
    private string _toolTip = "This is a Tool Tip.";
    private string _href = "test.html?Param1={0}&Param2={1}";
    private string _target = "_blank";
    private string _postBackEvent = "__doPostBack (\"Target\", \"Args\");";
    private string _onClick = "return false;";

    // construction and disposing

    public CommandTestHelper ()
    {
      _functionType = typeof (ExecutionEngine.TestFunction);
      _functionTypeName = WebTypeUtility.GetQualifiedName (_functionType);
    }

    // methods and properties

    public string ToolTip
    {
      get { return _toolTip; }
    }

    public string Href
    {
      get { return _href; }
    }

    public string WxeFunctionParameters
    {
      get { return _wxeFunctionParameters; }
    }

    public string Target
    {
      get { return _target; }
    }

    public string PostBackEvent
    {
      get { return _postBackEvent; }
    }

    public string OnClick
    {
      get { return _onClick; }
    }

    public Command CreateHrefCommand ()
    {
      Command command = new Command ();
      command.Type = CommandType.Href;
      command.ToolTip = _toolTip;
      command.HrefCommand.Href = _href;
      command.HrefCommand.Target = _target;

      return command;
    }

    public Command CreateEventCommand ()
    {
      Command command = new Command ();
      command.Type = CommandType.Event;
      command.ToolTip = _toolTip;

      return command;
    }

    public Command CreateWxeFunctionCommand ()
    {
      Command command = new Command ();
      command.Type = CommandType.WxeFunction;
      command.ToolTip = _toolTip;
      command.WxeFunctionCommand.TypeName = _functionTypeName;
      command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
      command.WxeFunctionCommand.Target = _target;

      return command;
    }

    public Command CreateNoneCommand ()
    {
      Command command = new Command ();
      command = new Command ();
      command.Type = CommandType.None;

      return command;
    }
  }
}