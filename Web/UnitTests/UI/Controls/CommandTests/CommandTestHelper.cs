using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.UI;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UnitTests.UI.Controls.CommandTests
{
  public class CommandTestHelper
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IWebSecurityProvider _mockWebSecurityProvider;
    private IWxeSecurityProvider _mockWxeSecurityProvider;

    private HttpContext _httpContext;
    private HtmlTextWriterSingleTagMock _htmlWriter;

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

      _mocks = new MockRepository ();
      _mockWebSecurityProvider = _mocks.CreateMock<IWebSecurityProvider> ();
      _mockWxeSecurityProvider = _mocks.CreateMock<IWxeSecurityProvider> ();

      _httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _httpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;

      _htmlWriter = new HtmlTextWriterSingleTagMock ();
    }

    // methods and properties

    public HttpContext HttpContext
    {
      get { return _httpContext; }
    }

    public HtmlTextWriterSingleTagMock HtmlWriter
    {
      get { return _htmlWriter; }
    }

    public IWebSecurityProvider WebSecurityProvider
    {
      get { return _mockWebSecurityProvider; }
    }

    public IWxeSecurityProvider WxeSecurityProvider
    {
      get { return _mockWxeSecurityProvider; }
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public void ExpectWebSecurityProviderHasAccess (ISecurableObject securableObject, Delegate handler, bool returnValue)
    {
      Expect.Call (_mockWebSecurityProvider.HasAccess (securableObject, handler)).Return (returnValue).Repeat.Once ();
    }

    public void ExpectWxeSecurityProviderHasStatelessAccess (Type functionType, bool returnValue)
    {
      Expect.Call (_mockWxeSecurityProvider.HasStatelessAccess (functionType)).Return (returnValue).Repeat.Once ();
    }


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
      InitializeHrefCommand (command);

      return command;
    }

    public Command CreateHrefCommandAsPartialMock ()
    {
      Command command = _mocks.PartialMock<Command> ();
      SetupResult.For (command.HrefCommand).CallOriginalMethod ();
      InitializeHrefCommand (command);

      return command;
    }

    private void InitializeHrefCommand (Command command)
    {
      command.Type = CommandType.Href;
      command.ToolTip = _toolTip;
      command.HrefCommand.Href = _href;
      command.HrefCommand.Target = _target;
    }

    public Command CreateEventCommand ()
    {
      Command command = new Command ();
      InitializeEventCommand (command);

      return command;
    }

    public Command CreateEventCommandAsPartialMock ()
    {
      Command command = _mocks.PartialMock<Command> ();
      InitializeEventCommand (command);

      return command;
    }

    private void InitializeEventCommand (Command command)
    {
      command.Type = CommandType.Event;
      command.ToolTip = _toolTip;
    }

    public Command CreateWxeFunctionCommand ()
    {
      Command command = new Command ();
      InitializeWxeFunctionCommand (command);

      return command;
    }

    public Command CreateWxeFunctionCommandAsPartialMock ()
    {
      Command command = _mocks.PartialMock<Command> ();
      SetupResult.For (command.WxeFunctionCommand).CallOriginalMethod();
      InitializeWxeFunctionCommand (command);

      return command;
    }

    private void InitializeWxeFunctionCommand (Command command)
    {
      command.Type = CommandType.WxeFunction;
      command.ToolTip = _toolTip;
      command.WxeFunctionCommand.TypeName = _functionTypeName;
      command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
      command.WxeFunctionCommand.Target = _target;
    }

    public Command CreateNoneCommand ()
    {
      Command command = new Command ();
      InitializeNoneCommand (command);

      return command;
    }

    public Command CreateNoneCommandAsPartialMock ()
    {
      Command command = _mocks.PartialMock<Command> ();
      InitializeNoneCommand (command);

      return command;
    }

    private void InitializeNoneCommand (Command command)
    {
      command.Type = CommandType.None;
    }

    public void ExpectOnceOnHasAccess (Command command, bool returnValue)
    {
      Expect.Call (command.HasAccess ()).Return (returnValue);
    }
  }
}