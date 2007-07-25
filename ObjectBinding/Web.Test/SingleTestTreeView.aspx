<%@ Register TagPrefix="cc1" Namespace="OBRTest" Assembly="OBRTest" %>
<%@ Page language="c#" Codebehind="SingleTestTreeView.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleTestTreeView" %>

<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.UI.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleTestTreeView</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
<body MS_POSITIONING="FlowLayout">
<form id=Form method=post runat="server">
<h1>SingleTest TreeView</h1>
<p><cc1:persontreeview id=PersonTreeView runat="server" DataSourceControl="CurrentObject" cssclass="TreeBlock" enabletoplevelexpander="False" enablelookaheadevaluation="True"></cc1:persontreeview><asp:Button id="RefreshPesonTreeViewButton" runat="server" Text="Refresh"></asp:Button></p>
<p>&nbsp;</p>
<p><rubicon:webtreeview id=WebTreeView runat="server" cssclass="TreeBlock" width="150px" enablescrollbars="True"></rubicon:webtreeview></p>
<p><asp:button id=PostBackButton runat="server" Text="PostBack"></asp:button></p><rubicon:formgridmanager 
id=FormGridManager 
runat="server"></rubicon:formgridmanager>
<p><rubicon:BindableObjectDataSourceControl 
id=CurrentObject runat="server" 
typename="OBRTest.Person, OBRTest" /></p>
<p><asp:label id=TreeViewLabel runat="server" EnableViewState="False">#</asp:label><asp:Button id="Node101Button" runat="server" Text="Node 101"></asp:Button></p></form>
	
  </body>
</html>
