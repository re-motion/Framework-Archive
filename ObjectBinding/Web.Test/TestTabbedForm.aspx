<%@ Register TagPrefix="obr" Namespace="Rubicon.ObjectBinding.Reflection" Assembly="Rubicon.ObjectBinding.Reflection" %>
<%@ Register TagPrefix="rwc" Namespace="Rubicon.Web.UI.Controls" Assembly="Rubicon.Web" %>
<%@ Register TagPrefix="obc" Namespace="Rubicon.ObjectBinding.Web.Controls" Assembly="Rubicon.ObjectBinding.Web" %>
<%@ Page language="c#" Codebehind="TestTabbedForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestTabbedForm" smartNavigation="True"%>
<%@ Register TagPrefix="iuc" TagName="CompleteBocTestUserControl" Src="CompleteBocTestUserControl.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>Test Tabbed Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><rwc:htmlheadcontents id=HtmlHeadContents runat="server"></rwc:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server">
<h1>Test Tabbed Form</h1>
<div style="HEIGHT: 2em"><asp:linkbutton id=SaveButton runat="server">Save</asp:linkbutton>
        &nbsp;
        <asp:linkbutton id="CancelButton" runat="server">Cancel</asp:linkbutton></div>
       <div>
      <rwc:tabstrip id="PagesTabStrip" runat="server" AutoPostBack="True" TabDefaultStyle="width:10em; height:1.6em; border-left:1px solid #000000; border-right:2px solid #000000; border-top:1px solid #000000; border-bottom:1px solid #000000; background-color:#E0E0E0; font-family:verdana,arial; font-weight:bold; font-size:.8em; color:#000000; text-align:center; "
        TabSelectedStyle="border-bottom:none;  background-color:#ffffff; color:#000000" TabHoverStyle="text-decoration: underline;  background-color:#E0E0E0; "
        SepDefaultStyle="border-bottom:1px solid #000000;  " Orientation="horizontal" TargetID="PagesMultiPage"
        width="100%"></rwc:tabstrip>
<table 
style="BORDER-RIGHT: black 2px solid; PADDING-LEFT: 1em; BORDER-LEFT: black 1px solid; WIDTH: 100%; BORDER-BOTTOM: black 2px solid; HEIGHT: 20em" 
cellpadding="0" ?>
  <tr>
    <td style="VERTICAL-ALIGN:top">
      <rwc:multipage id=PagesMultiPage runat="server" SelectedIndex="-1"></rwc:multipage></td></tr></table></div><asp:Button id="PostBackButton" runat="server" Text="PostBack"></asp:Button></form>
  </body>
</html>
