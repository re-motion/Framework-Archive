<%@ Register TagPrefix="obw" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="StartForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.StartForm" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Start Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema>
  </head>
<body MS_POSITIONING="FlowLayout">
<form id=Form method=post runat="server">
<p>Wxe-Enabled Tests for individual Business Object 
Controls<br><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.SingleBocTestMainWxeFunction,OBWTest" >WxeHandler.ashx?WxeFunctionType=OBWTest.SingleBocTestMainWxeFunction,OBWTest</A></p>
<p>Wxe-Enabled Tests containing all the Business Object 
Controls in a single Form or User Control<br><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest" >WxeHandler.ashx?WxeFunctionType=OBWTest.CompleteBocTestMainWxeFunction,OBWTest</A></p>
<p>Wxe-Enabled Test for a Tabbed Form<br><A href="WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&amp;ReadOnly=false" >WxeHandler.ashx?WxeFunctionType=OBWTest.TestTabbedFormWxeFunction,OBWTest&amp;ReadOnly=false</A></p>
<p>Test Tree View<br><A href="SingleTestTreeView.aspx" >SingleTestTreeView.aspx</A>
<rubicon:WebButton id="WebButton2" runat="server" text="&amp;Button" icon-url="Images/EditItem.gif"></rubicon:WebButton></p></form>
  </body>
</html>
