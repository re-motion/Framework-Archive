<%@ Page language="c#" Codebehind="SingleTestTreeView.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.SingleTestTreeView" %>
<%@ Register TagPrefix="rubicon" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>SingleTestTreeView</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <rubicon:htmlheadcontents id=HtmlHeadContents runat="server"></rubicon:htmlheadcontents>
  </head>
  <body MS_POSITIONING="FlowLayout">
	
    <form id="Form" method="post" runat="server">
<h1>SingleTest TreeView</h1>
<p><rubicon:WebTreeView id="WebTreeView" runat="server"></rubicon:WebTreeView></p>
<p><asp:Button id="PostBackButton" runat="server" Text="PostBack"></asp:Button></p>
<rubicon:FormGridManager id="FormGridManager" runat="server"></rubicon:FormGridManager>
<p><asp:Label id="TreeViewLabel" runat="server" EnableViewState="False">#</asp:Label></p>
    </form>
	
  </body>
</html>
