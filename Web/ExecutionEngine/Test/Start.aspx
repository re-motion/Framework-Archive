<%@ Page language="c#" Codebehind="Start.aspx.cs" AutoEventWireup="false" Inherits="Rubicon.PageTransition.Start" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>Start</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body MS_POSITIONING="FlowLayout">
    <form id="Form1" method="post" runat="server">
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.PageTransition.SampleWxeFunction,Rubicon.PageTransition">
          WxeHandler.ashx?WxeFunctionType=Rubicon.PageTransition.SampleWxeFunction,Rubicon.PageTransition
        </a>
      </p>
      <p>
        <a href="WxeHandler.ashx?WxeFunctionType=Rubicon.PageTransition.SessionWxeFunction,Rubicon.PageTransition&ReadOnly=True">
          WxeHandler.ashx?WxeFunctionType=Rubicon.PageTransition.SessionWxeFunction,Rubicon.PageTransition&ReadOnly=True
        </a>
      </p>
      <p>
        <a href="session.wxe?ReadOnly=True">session.wxe?ReadOnly=True
        </a>
      </p>
      <asp:Button id="ResetSessionButton" runat="server" Text="Reset Session"></asp:Button>
    </form>
  </body>
</HTML>
