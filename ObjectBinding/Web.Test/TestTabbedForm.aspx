<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm" smartNavigation="True"%>
<%@ Register TagPrefix="iuc" TagName="CompleteBocTestUserControl" Src="CompleteBocTestUserControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head><title>Test Tabbed Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
</head>
<body>
<form id=Form method=post runat="server">
<h1>Test Tabbed Form</h1>
<div style="HEIGHT: 2em"><asp:linkbutton id=SaveButton runat="server">Save</asp:linkbutton>&nbsp; 
<asp:linkbutton id=CancelButton runat="server">Cancel</asp:linkbutton></div><rwc:validationstateviewer id=ValidationStateViewer runat="server" DESIGNTIMEDRAGDROP="119" visible="true"></rwc:validationstateviewer>
<rwc:webtabstrip id=PagesTabStrip runat="server" width="100%" tabspanesize="4"></rwc:webtabstrip>
<p></p>
<rwc:tabbedMultiView id=MultiView runat="server">
</rwc:tabbedMultiView>
<asp:button id=PostBackButton runat="server" Text="PostBack"></asp:button>
<asp:button id=ValidateButton runat="server" Text="Validate"></asp:button></form></FORM>
  </body>
</html>
