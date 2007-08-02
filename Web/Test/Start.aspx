<%@ Page language="c#" Codebehind="Start.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.Web.Test.Start" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Start</title>
  </head>
  <body>
    <form id="Form1" method="post" runat="server">
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.Web.Test::ExecutionEngine.SampleWxeFunction">
          WxeHandler.ashx?WxeFunctionType=Rubicon.Web.Test::ExecutionEngine.SampleWxeFunction
        </a>
      </p>
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.Web.Test::ExecutionEngine.SessionWxeFunction&amp;ReadOnly=True">
          WxeHandler.ashx?WxeFunctionType=Rubicon.Web.Test::ExecutionEngine.SessionWxeFunction&amp;ReadOnly=True
        </a>
      </p>
      <p>
        <a href="session.wxe?ReadOnly=True">session.wxe?ReadOnly=True</a>
      </p>
      <p>
        <a href="MultiplePostbackCatching/SutForm.aspx">Multiple Postback Catcher Tests</a>
      </p>
      <p>
        <a href="../../Selenium/Core/TestRunner.html?test=../../ExecutionEngine/Test/TestSuite.aspx">Rubicon.Web.Tests</a>
      </p>
      <p>
        <a href="redirected.wxe">redirected.wxe</a>
      </p>
      <asp:Button id="ResetSessionButton" runat="server" Text="Reset Session"></asp:Button>
    </form>
  </body>
</html>
