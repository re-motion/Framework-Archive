<%@ Register TagPrefix="iuc" TagName="CompleteBocTestUserControl" Src="CompleteBocTestUserControl.ascx" %>
<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm" smartNavigation="True"%>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head><title>Test Tabbed Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
<style>SepDefaultStyle { BORDER-BOTTOM: #000000 1px solid }
	TabHoverStyle { BACKGROUND-COLOR: #e0e0e0; TEXT-DECORATION: underline }
	TabSelectedStyle { COLOR: #000000; BORDER-BOTTOM: medium none; BACKGROUND-COLOR: #ffffff }
	TabDefaultStyle { BORDER-RIGHT: #000000 2px solid; BORDER-TOP: #000000 1px solid; FONT-WEIGHT: bold; FONT-SIZE: 0.8em; BORDER-LEFT: #000000 1px solid; WIDTH: 10em; COLOR: #000000; BORDER-BOTTOM: #000000 1px solid; FONT-FAMILY: verdana,arial; HEIGHT: 1.6em; BACKGROUND-COLOR: #e0e0e0; TEXT-ALIGN: center }
	</style>
</head>
<body>
<form id=Form method=post runat="server">
<h1>Test Tabbed Form</h1>
<div style="HEIGHT: 2em"><asp:linkbutton id=SaveButton runat="server">Save</asp:linkbutton>&nbsp; 
<asp:linkbutton id=CancelButton runat="server">Cancel</asp:linkbutton></div><rwc:validationstateviewer id=ValidationStateViewer runat="server" DESIGNTIMEDRAGDROP="119" visible="true"></rwc:validationstateviewer>
<div><rwc:webtabstrip id=PagesTabStrip runat="server" width="100%" tabspanesize="4"></rwc:webtabstrip>
<table 
style="BORDER-RIGHT: black 2px solid; PADDING-LEFT: 1em; BORDER-LEFT: black 1px solid; WIDTH: 100%; BORDER-BOTTOM: black 2px solid; HEIGHT: 20em" 
cellPadding=0>
  <tr>
    <td style="VERTICAL-ALIGN: top"><rwc:multipage id=PagesMultiPage runat="server" SelectedIndex="-1"></rwc:multipage></td></tr></table></div><asp:button id=PostBackButton runat="server" Text="PostBack"></asp:button><asp:button id=ValidateButton runat="server" Text="Validate"></asp:button></form>
<div></div></FORM>
  </body>
</html>
