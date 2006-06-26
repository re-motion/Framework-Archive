using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using NMock2;

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

    private Mockery _mocks;
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

      _mocks = new Mockery ();
      _mockWebSecurityProvider = _mocks.NewMock<IWebSecurityProvider> ();
      _mockWxeSecurityProvider = _mocks.NewMock<IWxeSecurityProvider> ();

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
      get { return _htmlWriter;}
    }

    public IWebSecurityProvider WebSecurityProvider
    {
      get { return _mockWebSecurityProvider; }
    }

    public IWxeSecurityProvider WxeSecurityProvider
    {
      get { return _mockWxeSecurityProvider; }
    }

    public void VerifyAllExpectationsHaveBeenMet ()
    {
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    public void ExpectWebSecurityProviderToBeNeverCalled ()
    {
      Expect.Never.On (_mockWebSecurityProvider);
    }

    public void ExpectWxeSecurityProviderToBeNeverCalled ()
    {
      Expect.Never.On (_mockWxeSecurityProvider);
    }

    public void ExpectWebSecurityProviderHasAccess (ISecurableObject securableObject, Delegate handler, bool returnValue)
    {
      Expect.Once.On (_mockWebSecurityProvider)
         .Method ("HasAccess")
         .With (securableObject, handler)
         .Will (Return.Value (returnValue));
    }

    public void ExpectWxeSecurityProviderHasStatelessAccess (Type functionType, bool returnValue)
    {
      Expect.Once.On (_mockWxeSecurityProvider)
         .Method ("HasStatelessAccess")
         .With (functionType)
         .Will (Return.Value (returnValue));
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

    public TestCommand CreateHrefCommand ()
    {
      TestCommand command = new TestCommand ();
      command.Type = CommandType.Href;
      command.ToolTip = _toolTip;
      command.HrefCommand.Href = _href;
      command.HrefCommand.Target = _target;

      return command;
    }

    public TestCommand CreateEventCommand ()
    {
      TestCommand command = new TestCommand ();
      command.Type = CommandType.Event;
      command.ToolTip = _toolTip;

      return command;
    }

    public TestCommand CreateWxeFunctionCommand ()
    {
      TestCommand command = new TestCommand ();
      command.Type = CommandType.WxeFunction;
      command.ToolTip = _toolTip;
      command.WxeFunctionCommand.TypeName = _functionTypeName;
      command.WxeFunctionCommand.Parameters = _wxeFunctionParameters;
      command.WxeFunctionCommand.Target = _target;

      return command;
    }

    public TestCommand CreateNoneCommand ()
    {
      TestCommand command = new TestCommand ();
      command.Type = CommandType.None;

      return command;
    }
  }
}