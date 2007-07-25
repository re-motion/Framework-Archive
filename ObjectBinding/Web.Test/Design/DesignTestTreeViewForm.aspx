<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="cc1" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Page language="c#" Codebehind="DesignTestTreeViewForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestTreeViewForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: TreeView Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><rwc:webbutton id=PostBackButton runat="server" Text="PostBack"></rwc:webbutton>
<h1>DesignTest: TreeView Form</h1>
<p><cc1:persontreeview id="PersonTreeView" runat="server" cssclass="TreeBlock" DataSourceControl="CurrentObject" enabletoplevelexpander="False" enablelookaheadevaluation="True"></cc1:persontreeview></p>
<p>

<obr:CurrentObject id="CurrentObject" runat="server" typename="OBRTest.Person, OBRTest"></obr:CurrentObject></p></form>
  </body>
</html>
