<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Page language="c#" Codebehind="SingleTestTreeView.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleTestTreeView" %>
<%@ Register TagPrefix="cc1" Namespace="OBRTest" Assembly="OBRTest" %>
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
<h1>SingleTest TreeView</H1>
<p><cc1:persontreeview id=PersonTreeView runat="server" EnableTopLevelExpander="False" DataSourceControl="ReflectionBusinessObjectDataSourceControl" cssclass="TreeBlock"></cc1:persontreeview></P>
<p>&nbsp;</P>
<p><rubicon:webtreeview id=WebTreeView runat="server" cssclass="TreeBlock" showlines="False"></rubicon:webtreeview></P>
<p><asp:button id=PostBackButton runat="server" Text="PostBack"></asp:button></P><rubicon:formgridmanager 
id=FormGridManager 
runat="server"></rubicon:formgridmanager>
<p><obr:reflectionbusinessobjectdatasourcecontrol 
id=ReflectionBusinessObjectDataSourceControl runat="server" 
typename="OBRTest.Person, OBRTest"></obr:reflectionbusinessobjectdatasourcecontrol></P>
<p><asp:label id=TreeViewLabel runat="server" EnableViewState="False">#</asp:label></P></FORM>
	
  </body>
</html>
